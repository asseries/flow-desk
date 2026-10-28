# Qisqa qo‘llanma

Flow Desk — haqiqiy Windows ilovalarini tablar va panellarda boshqaradigan ochiq kodli dastur.

## Boshlash

1. [Releases](https://github.com/asseries/flow-desk/releases/latest) sahifasidan `FlowDesk-Setup-win-x64.exe`ni yuklab, ishga tushiring. Administrator huquqi talab qilinmaydi.
2. Xohlasangiz Windowsga kirganda avtomatik boshlash va Desktop yorlig‘ini belgilang.
3. Start menyusidan **Flow Desk**ni oching. Portable ZIP ham mavjud: uni to‘liq ochib, `FlowDesk.exe`ni ishga tushirish mumkin.
4. **☰ → Add window…** orqali ochiq ilovalarni tanlang yoki **Run app…** orqali ilova ishga tushiring.
5. **Split** orqali yonma-yon yoki yuqori/past panel yarating; chegarasini sudrab o‘lchamini o‘zgartiring.

Tabdagi **×** ilovani yopmaydi — uni guruhdan chiqaradi. Ilova oynasini yopish alohida kontekst-menyu amalidir.

O‘rnatilgan dastur Windows **Installed apps** ro‘yxatida chiqadi va o‘sha yerdan o‘chiriladi. Saqlangan workspace ma’lumotlari o‘chirilmaydi. Avtozapusk `--background` orqali faqat tray rejimida boshlaydi.

Workspace’lar vaqtinchalik. Qayta ishlatish uchun **☰ → Workspaces → Save workspace…** orqali saqlang. Ulanmagan eski nomlar bo‘sh qora tab sifatida chiqarilmaydi.

## Boshqa virtual desktop

Add window’da boshqa desktop oynalarini ko‘rsatish dastlabdan belgilangan. Tanlangan oynani lokal adapter shu desktopga olib kelishga urinadi. Ajratilganda asl desktopiga qaytarish ma’lumoti saqlanadi. Windows yangilanishi yoki himoyalangan ilova sabab ko‘chirish ishlamasligi mumkin; bunday holatda Task View orqali qo‘lda ko‘chirish mumkin.

## Oyna va tray

- Tepada bo‘sh tab qatoridan sudrab oynani ko‘chiring.
- Minimize, maximize/restore, full screen va yopish — **☰** ichida.
- Flow Desk fokusda bo‘lganda **F11** to‘liq ekran, **Ctrl+Tab** tab, **F6** panel almashtiradi.
- Oxirgi workspace yopilganda dastur tray’da qoladi. To‘liq chiqish uchun **Exit Flow Desk**.
- Windows bilan fon rejimida boshlash uchun Startup papkasiga `FlowDesk.exe --background` yorlig‘ini qo‘ying.

## Koddan build

```powershell
.\build.ps1
.\dist\FlowDesk.exe
.\test.ps1 -ModelOnly
```

To‘liq testlar: `test.ps1`; interaktiv Windows sessiyasi va kamida ikkita virtual desktop kerak.

Asosiy rejim — **Aligned**. **Embedded** tajribaviy: har bir dastur bilan alohida tekshirish kerak. Bu dastur universal konteyner yoki barcha ilovalar uchun kafolatlangan embedding yechimi emas.
