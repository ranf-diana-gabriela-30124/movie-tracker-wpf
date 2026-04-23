# 🎬 CinemaGlow

**CinemaGlow** este o aplicație desktop modernă construită cu **WinUI 3**, concepută pentru cei care doresc să își gestioneze colecția de filme într-un mod elegant și organizat. De la plănuirea vizionărilor viitoare până la monitorizarea activității de admin, CinemaGlow oferă o experiență vizuală fluidă și performantă.

![WinUI 3](https://img.shields.io/badge/UI-WinUI%203-blue?style=for-the-badge&logo=windows)
![C#](https://img.shields.io/badge/Language-C%23-purple?style=for-the-badge&logo=csharp)
![SQLite](https://img.shields.io/badge/Database-SQLite-003B57?style=for-the-badge&logo=sqlite)

---

## 🖼️ Preview & Screenshots

### 🖥️ Autentificare
| Log In | Sign Up |
| :---: | :---: |
| ![Log In](/UI_Photos/LogIn.jpg) | ![Sign Up](/UI_Photos/CreateAccount.jpg) |

### 🖥️ Dashboard Utilizator
Interfața principală unde poți gestiona lista de filme și poți folosi funcția de Random Picker.
![User Dashboard](/UI_Photos/HomePage.jpg)

### 📊 Admin Panel
Secțiunea dedicată administratorilor pentru monitorizarea sesiunilor și a activității utilizatorilor.
![Admin Panel](/UI_Photos/AdminPanel.jpg)

### ➕ Dialoguri Interactive
Sistem de ferestre modale pentru adăugarea, editarea și notarea filmelor vizionate.
| Add Movie to WatchList | Mark movie as watched |
| :---: | :---: |
| ![AddWatchList](/UI_Photos/DialogBoxAddWatchList) | ![MarkedAsWatched](/UI_Photos/DialogBoxMarkedAsWatched) |




---

## ✨ Funcționalități Cheie

### 🍿 Pentru Utilizatori:
* **Smart Watchlist:** Adaugă filmele pe care vrei să le vezi cu un singur click.
* **Watched History:** Arhivează filmele văzute, adaugă rating-uri cu stele, platforma de vizionare și data.
* **Random Picker:** Te simți indecis? Lasă aplicația să aleagă un film pentru tine din lista ta, filtrat pe genul preferat.
* **Dark Aesthetic:** Design modern cu accente "Gold" și fundaluri blurate pentru o experiență imersivă.

### 🔐 Pentru Administratori:
* **Sessions Log:** Monitorizare completă: cine s-a logat, la ce oră și cât a durat sesiunea.
* **Database Management:** Vizualizarea centralizată a tuturor datelor prin SQLite.

---

## 🛠️ Detalii Tehnice

| Tehnologie | Utilizare |
| :--- | :--- |
| **WinUI 3** | Interfața grafică modernă (Windows App SDK). |
| **SQLite** | Stocarea locală a filmelor, utilizatorilor și sesiunilor. |
| **Entity Logic** | Arhitectură bazată pe Repository Pattern pentru cod curat. |
| **XAML** | Design declarativ pentru ferestre și stilizarea elementelor. |

---

## 📂 Structura Bazei de Date

Aplicația folosește o structură relațională simplă dar eficientă:
* `Users` - Datele de autentificare și rolurile (Admin/User).
* `Watchlist` - Lista de filme "To-Watch".
* `WatchedMovies` - Istoricul complet cu Rating și Platformă.
* `Sessions` - Log-urile de Login/Logout.
