"""HTTP + SQL integration checks. Only accepts a disposable QLSanBongWeb_Test_* database.
Start the website with its connection string pointed to that same database first.
Requires Python 3 and sqlcmd; uses only the Python standard library.
"""
import argparse, concurrent.futures, datetime, html, http.cookiejar, json, re, subprocess, sys, time
from pathlib import Path
from urllib import request, parse, error

parser = argparse.ArgumentParser()
parser.add_argument('--url', default='http://localhost:5086')
parser.add_argument('--database', required=True)
parser.add_argument('--server', default=r'.\SQLEXPRESS')
parser.add_argument('--node', help='Optional Node.js executable for syntax checking rendered inline scripts')
args = parser.parse_args()
if not re.fullmatch(r'QLSanBongWeb_Test_[A-Za-z0-9_]+', args.database):
    raise SystemExit('Refusing to test against a non-test database')
results=[]
def check(name, ok, detail=''):
    results.append({'name':name,'passed':bool(ok),'detail':detail})
    print(('PASS ' if ok else 'FAIL ')+name, flush=True)
    if not ok: raise AssertionError(name+': '+detail)
def sql(query):
    proc=subprocess.run(['sqlcmd','-S',args.server,'-E','-C','-d',args.database,'-b','-f','65001','-W','-h','-1','-s','|','-Q','SET NOCOUNT ON; '+query],capture_output=True,text=True,encoding='utf-8',errors='replace')
    if proc.returncode: raise RuntimeError(proc.stdout+proc.stderr)
    return proc.stdout.strip()
def literal(text): return "N'"+str(text).replace("'","''")+"'"
class NoRedirect(request.HTTPRedirectHandler):
    def redirect_request(self,*unused): return None
class Client:
    def __init__(self):
        self.jar=http.cookiejar.CookieJar()
        self.opener=request.build_opener(request.HTTPCookieProcessor(self.jar),NoRedirect())
    def send(self,path,fields=None,ajax=False):
        data=None if fields is None else parse.urlencode(fields).encode()
        headers={'Accept':'application/json' if ajax else 'text/html'}
        req=request.Request(args.url+path,data=data,headers=headers)
        try: response=self.opener.open(req,timeout=25)
        except error.HTTPError as e: response=e
        self.raw_body=response.read().decode('utf-8',errors='replace')
        return response.code,html.unescape(self.raw_body),dict(response.headers)
    def token(self,path='/'):
        code,body,_=self.send(path)
        if code != 200: raise RuntimeError(f'Cannot obtain token at {path}: {code} {body[:300]}')
        match=re.search(r'<input[^>]*name="__RequestVerificationToken"[^>]*value="([^"]+)"',body)
        if not match: raise RuntimeError('Missing antiforgery token at '+path)
        return match.group(1)
    def post(self,path,fields,token_path='/',ajax=False):
        fields=dict(fields);fields['__RequestVerificationToken']=self.token(token_path)
        return self.send(path,fields,ajax)
    def login(self,username='admin_demo',password='Admin@123'):
        return self.post('/Account/Login',{'tenDangNhap':username,'matKhau':password},'/Account/Login')
try:
    anon=Client()
    for path in ('/','/San','/DatSan','/KhachHang','/HoaDon','/ThongKe'):
        code,body,headers=anon.send(path)
        check('Anonymous blocked: '+path,code==302 and '/Account/Login' in headers.get('Location',''))
    check('Login POST requires antiforgery',anon.send('/Account/Login',{'tenDangNhap':'admin_demo','matKhau':'x'})[0]==400)
    check('Incorrect password rejected',anon.login(password='incorrect')[0]==200)
    admin=Client();check('PBKDF2 manager login',admin.login()[0]==302)
    for path in ('/','/San','/San/Create','/San/Edit/S001','/KhachHang','/DatSan','/DatSan/Create','/HoaDon','/ThongKe','/ThanhToan?maHD=HD004'):
        code,body,_=admin.send(path)
        check('Render '+path,code==200,str(code)+' '+body[:100])
        if args.node:
            scripts='\n'.join(re.findall(r'<script\b[^>]*>(.*?)</script>',admin.raw_body,re.S|re.I))
            if scripts.strip():
                syntax=subprocess.run([args.node,'--check','-'],input=scripts,capture_output=True,text=True,encoding='utf-8')
                check('JavaScript syntax '+path,syntax.returncode==0,syntax.stderr[:200])
        if path=='/DatSan/Create':
            scripts='\n'.join(re.findall(r'<script\b[^>]*>(.*?)</script>',body,re.S|re.I))
            check('Booking script has repaired date assignment','value; "' not in scripts and 'capNhatGia' in scripts)
    code,body,_=admin.send('/ThongKe?tuNgay=2026-10-15&denNgay=2026-10-15')
    check('Sample revenue is 970000',code==200 and re.search(r'970[.,]000',body) is not None)
    check('Revenue rejects reversed dates','không hợp lệ' in admin.send('/ThongKe?tuNgay=2026-11-02&denNgay=2026-11-01')[1])
    check('Mutation requires antiforgery',admin.send('/San/Create',{'MaSan':'QA_BAD'})[0]==400)
    invalid={'MaSan':'QA_BAD','TenSan':'Invalid','LoaiSan':'Sân 5 người','GiaThueGio':'0','TrangThaiHoatDong':'true'}
    check('Zero pitch price rejected',admin.post('/San/Create',invalid)[0]==200 and sql("SELECT COUNT(*) FROM dbo.tblSan WHERE MaSan='QA_BAD'")=='0')
    pitch=dict(invalid,MaSan='QA_SAN',TenSan='Sân kiểm thử',GiaThueGio='200000')
    check('Create pitch',admin.post('/San/Create',pitch)[0]==302)
    check('Duplicate pitch handled',admin.post('/San/Create',pitch)[0]==200)
    check('Edit pitch',admin.post('/San/Edit',dict(pitch,TenSan='Sân kiểm thử cập nhật'))[0]==302)
    check('Invalid customer rejected and values retained','Số điện thoại' in admin.post('/KhachHang/Save',{'MaKH':'KH_TEMP','TenKH':"O'Neil <test>",'SoDT':'abc','actionType':'Add'})[1])
    code,_,_=admin.post('/KhachHang/Save',{'MaKH':'KH_TEMP','TenKH':"O'Neil <test>",'SoDT':'0999999999','actionType':'Add'})
    check('Create customer',code==302)
    customer=sql("SELECT MaKH FROM dbo.tblKhachHang WHERE SoDT='0999999999'")
    check('Edit customer',admin.post('/KhachHang/Save',{'MaKH':customer,'TenKH':"O'Neil <test>",'SoDT':'0988888888','actionType':'Edit'})[0]==302)
    code,body,_=admin.send('/KhachHang')
    check('Customer names encoded in data attributes','class="chon-khach"' in body and 'onclick="chonKhachHang(' not in body and '&lt;test&gt;' in admin.raw_body)
    today=datetime.datetime.strptime(sql('SELECT CONVERT(char(10),GETDATE(),23)'),'%Y-%m-%d').date()
    day=today+datetime.timedelta(days=3)
    def booking(start,end,pitch='QA_SAN',day_value=day):
        return {'MaKH':customer,'MaSan':pitch,'GioBatDau':f'{day_value}T{start}:00','GioKetThuc':f'{day_value}T{end}:00'}
    check('Inactive pitch rejected',admin.post('/DatSan/Create',booking('17:00','18:00','S004'))[0]==200)
    check('Reversed booking time rejected',admin.post('/DatSan/Create',booking('19:00','18:00'))[0]==200)
    check('Past booking rejected',admin.post('/DatSan/Create',booking('17:00','18:00',day_value=today-datetime.timedelta(days=1)))[0]==200)
    check('Missing pitch rejected',admin.post('/DatSan/Create',dict(booking('17:00','18:00'),MaSan=''))[0]==200)
    check('90-minute booking saved',admin.post('/DatSan/Create',booking('17:00','18:30'))[0]==302)
    booked=sql("SELECT MaDatSan FROM dbo.tblDatSan WHERE MaSan='QA_SAN' AND CONVERT(date,GioBatDau)="+literal(day)+" AND CONVERT(time,GioBatDau)='17:00'")
    check('Overlapping booking rejected',admin.post('/DatSan/Create',booking('18:00','19:00'))[0]==200)
    check('Adjacent booking allowed',admin.post('/DatSan/Create',booking('18:30','19:00'))[0]==302)
    adjacent=sql("SELECT MaDatSan FROM dbo.tblDatSan WHERE MaSan='QA_SAN' AND CONVERT(date,GioBatDau)="+literal(day)+" AND CONVERT(time,GioBatDau)='18:30'")
    code,body,_=admin.send('/DatSan?ngayXem='+str(day)+'&maSan=QA_SAN')
    check('Dynamic pitch and partial-hour cells render',code==200 and 'QA_SAN' in body and '18:00' in body and booked in body and 'data-details=' in body)
    check('Cancel eligible booking',admin.post('/DatSan/HuyLich',{'id':adjacent,'ngayXem':str(day)})[0]==302 and sql('SELECT TrangThaiDat FROM dbo.tblDatSan WHERE MaDatSan='+literal(adjacent))=='Đã hủy')
    code,body,_=admin.post('/HoaDon/LapHoaDon',{'maDatSan':adjacent},'/HoaDon',True)
    check('Invoice rejects cancelled booking',code==200 and json.loads(body)['success'] is False)
    detail=json.loads(admin.send('/HoaDon/LayThongTinDatSan?maDatSan='+booked,ajax=True)[1])
    check('90-minute invoice preview = 300000',detail['tongTien']==300000)
    code,body,_=admin.post('/HoaDon/LapHoaDon',{'maDatSan':booked},'/HoaDon',True)
    invoice=json.loads(body)
    check('Create invoice',code==200 and invoice.get('success') is True,body[:200])
    invoice_id=invoice['maHD']
    duplicate=json.loads(admin.post('/HoaDon/LapHoaDon',{'maDatSan':booked},'/HoaDon',True)[1])
    check('Duplicate invoice rejected',duplicate.get('success') is False)
    admin.post('/DatSan/HuyLich',{'id':booked,'ngayXem':str(day)})
    check('Cannot cancel invoiced booking',sql('SELECT TrangThaiDat FROM dbo.tblDatSan WHERE MaDatSan='+literal(booked))=='Đã đặt')
    admin.post('/San/Edit',dict(pitch,GiaThueGio='250000'))
    check('Invoice and booking retain historical price',sql('SELECT CAST(h.TongTien AS int),CAST(d.DonGiaApDung AS int) FROM dbo.tblHoaDon h JOIN dbo.tblDatSan d ON h.MaDatSan=d.MaDatSan WHERE h.MaHD='+literal(invoice_id))=='300000|200000')
    code,body,_=admin.post('/ThanhToan/XacNhanThanhToan',{'maHD':invoice_id,'phuongThuc':'Invalid'})
    check('Invalid payment method rejected',code==400)
    check('Confirm payment',admin.post('/ThanhToan/XacNhanThanhToan',{'maHD':invoice_id,'phuongThuc':'TienMat'})[0]==302)
    first=sql('SELECT TrangThaiThanhToan,CONVERT(varchar(30),ThoiDiemThanhToan,126) FROM dbo.tblHoaDon WHERE MaHD='+literal(invoice_id))
    time.sleep(1.1)
    admin.post('/ThanhToan/XacNhanThanhToan',{'maHD':invoice_id,'phuongThuc':'TienMat'})
    check('Repeat payment preserves timestamp',first==sql('SELECT TrangThaiThanhToan,CONVERT(varchar(30),ThoiDiemThanhToan,126) FROM dbo.tblHoaDon WHERE MaHD='+literal(invoice_id)) and first.startswith('1|'))
    revenue_page=admin.send('/ThongKe?tuNgay='+str(today)+'&denNgay='+str(today))[1]
    match=re.search(r'Tổng doanh thu:\s*([0-9.,]+)',revenue_page)
    check('Paid invoice contributes revenue',match is not None and int(re.sub(r'[^0-9]','',match.group(1)))==300000)
    staff=Client();check('Staff login',staff.login('nhanvien_demo','Nv@12345')[0]==302)
    for path in ('/San/Create','/San/Edit/S001','/ThongKe'):
        check('Staff forbidden '+path,staff.send(path)[0]==403)
    check('Staff cannot POST pitch',staff.post('/San/Create',dict(pitch,MaSan='QA_FORBIDDEN'))[0]==403)
    check('Staff can view bookings',staff.send('/DatSan')[0]==200)
    check('Staff can access billing',staff.send('/HoaDon')[0]==200)
    sql("INSERT dbo.tblTaiKhoan SELECT 'QA_LOCKED',N'qa_locked',MatKhauHash,N'Quản lý',0 FROM dbo.tblTaiKhoan WHERE TenDangNhap=N'admin_demo'")
    check('Locked account rejected',Client().login('qa_locked','Admin@123')[0]==200)
    # Two different browser sessions target the same pitch/slot simultaneously.
    second=Client();check('Second manager session',second.login()[0]==302)
    pair=[admin,second]
    tokens=[c.token('/DatSan/Create') for c in pair]
    fields=booking('20:00','21:00')
    def race(i): return pair[i].send('/DatSan/Create',dict(fields,__RequestVerificationToken=tokens[i]))[0]
    with concurrent.futures.ThreadPoolExecutor(max_workers=2) as pool:
        statuses=list(pool.map(race,range(2)))
    count=sql("SELECT COUNT(*) FROM dbo.tblDatSan WHERE MaSan='QA_SAN' AND CONVERT(date,GioBatDau)="+literal(day)+" AND CONVERT(time,GioBatDau)='20:00' AND TrangThaiDat<>N'Đã hủy'")
    check('Concurrent duplicate booking: exactly one saved',count=='1' and sorted(statuses)==[200,302],str(statuses)+' count='+count)
    race_id=sql("SELECT MaDatSan FROM dbo.tblDatSan WHERE MaSan='QA_SAN' AND CONVERT(date,GioBatDau)="+literal(day)+" AND CONVERT(time,GioBatDau)='20:00'")
    tokens=[c.token('/HoaDon') for c in pair]
    def invoice_race(i): return pair[i].send('/HoaDon/LapHoaDon',{'maDatSan':race_id,'__RequestVerificationToken':tokens[i]},True)
    with concurrent.futures.ThreadPoolExecutor(max_workers=2) as pool:
        responses=list(pool.map(invoice_race,range(2)))
    successes=[json.loads(body).get('success') for code,body,_ in responses if code==200]
    check('Concurrent invoice: exactly one saved',successes.count(True)==1 and successes.count(False)==1 and sql('SELECT COUNT(*) FROM dbo.tblHoaDon WHERE MaDatSan='+literal(race_id))=='1',str([r[0] for r in responses]))
    race_invoice=sql('SELECT MaHD FROM dbo.tblHoaDon WHERE MaDatSan='+literal(race_id))
    tokens=[c.token('/') for c in pair]
    def payment_race(i): return pair[i].send('/ThanhToan/XacNhanThanhToan',{'maHD':race_invoice,'phuongThuc':'TienMat','__RequestVerificationToken':tokens[i]})[0]
    with concurrent.futures.ThreadPoolExecutor(max_workers=2) as pool:
        statuses=list(pool.map(payment_race,range(2)))
    check('Concurrent payment completes without duplicate update',statuses==[302,302] and sql('SELECT COUNT(*) FROM dbo.tblHoaDon WHERE MaHD='+literal(race_invoice)+' AND TrangThaiThanhToan=1 AND ThoiDiemThanhToan IS NOT NULL')=='1',str(statuses))
    check('Create cancellation/invoice race fixture',admin.post('/DatSan/Create',booking('22:00','23:00'))[0]==302)
    cancel_id=sql("SELECT MaDatSan FROM dbo.tblDatSan WHERE MaSan='QA_SAN' AND CONVERT(date,GioBatDau)="+literal(day)+" AND CONVERT(time,GioBatDau)='22:00'")
    tokens=[c.token('/') for c in pair]
    def cancel_invoice_race(i):
        if i==0: return pair[i].send('/DatSan/HuyLich',{'id':cancel_id,'ngayXem':str(day),'__RequestVerificationToken':tokens[i]})[0]
        return pair[i].send('/HoaDon/LapHoaDon',{'maDatSan':cancel_id,'__RequestVerificationToken':tokens[i]},True)[0]
    with concurrent.futures.ThreadPoolExecutor(max_workers=2) as pool:
        statuses=list(pool.map(cancel_invoice_race,range(2)))
    state=sql('SELECT d.TrangThaiDat,COUNT(h.MaHD) FROM dbo.tblDatSan d LEFT JOIN dbo.tblHoaDon h ON d.MaDatSan=h.MaDatSan WHERE d.MaDatSan='+literal(cancel_id)+' GROUP BY d.TrangThaiDat')
    check('Cancel versus invoice race stays consistent',state in ('Đã hủy|0','Đã đặt|1') and statuses==[302,200],state+' '+str(statuses))
    overnight=booking('23:30','00:30')
    overnight['GioKetThuc']=f'{day+datetime.timedelta(days=1)}T00:30:00'
    check('Overnight booking accepted by server',admin.post('/DatSan/Create',overnight)[0]==302)
    overnight_id=sql("SELECT MaDatSan FROM dbo.tblDatSan WHERE MaSan='QA_SAN' AND CONVERT(date,GioBatDau)="+literal(day)+" AND CONVERT(time,GioBatDau)='23:30'")
    check('Overlap across midnight rejected',admin.post('/DatSan/Create',booking('00:00','01:00',day_value=day+datetime.timedelta(days=1)))[0]==200)
    check('Overnight booking visible on following day',overnight_id in admin.send('/DatSan?ngayXem='+str(day+datetime.timedelta(days=1))+'&maSan=QA_SAN')[1])
    check('Logout',admin.post('/Account/Logout',{})[0]==302)
    check('Logged-out session blocked',admin.send('/HoaDon')[0]==302)
except Exception as exc:
    print(type(exc).__name__+': '+str(exc),file=sys.stderr)
    if not results or results[-1]['passed']:
        results.append({'name':'Unexpected error','passed':False,'detail':str(exc)})
finally:
    output=Path(__file__).resolve().parent/'http-results.json'
    output.write_text(json.dumps({'database':args.database,'tested_at':datetime.datetime.now().isoformat(),'checks':results},ensure_ascii=False,indent=2),encoding='utf-8')
    print(f'{sum(r["passed"] for r in results)}/{len(results)} passed; {output}',flush=True)
sys.exit(0 if results and all(r['passed'] for r in results) else 1)
