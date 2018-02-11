# Masakin-Backend

Untuk penggunaan API, kami menggunakan 3 API dari XSight, yaitu:

- SMSOTP, untuk memverifikasi pengguna yang mendaftar di platform Masakin

- SMSNotification, untuk memberitahukan pengguna ketika pesanannya diproses, dikirimkan, dan pesanan/kateringnya sudah selesai, dan digunakan juga untuk dapur mitra kami ketika ada pengguna yang memesan katering mereka.

- Finpay, digunakan untuk pengguna meakukan pembayaran atas pesanan katering mereka.

untuk penggunaan API XSight, ada di file TransactionController.cs dan AuthenticationController.cs dan untuk memproses API XSightnya ada di file Ashley/Common.cs


Untuk download APKnya di link ini: https://www.dropbox.com/s/4ccotibn4vyohw1/Masakin%20%28beta%29.apk?dl=0

Untuk keperluan testing, berikut user dan merchant id:
- User (atau bisa bikin user baru dengan mendaftar di aplikasi Masakin)

  Phone : 089508547770
  
  Password : 12345
 
- Merchant

  Phone : 085656474530
  
  Password : 12345
  
