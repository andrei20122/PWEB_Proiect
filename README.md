# Server Student Accommodation
Am dezvoltat în ASP.NET Core un server pentru o aplicație web care gestionează cazarea studenților în cămine.
Sistemul oferă funcționalități complete de la aplicarea pentru locuri de cazare până la gestionarea documentelor studentilor si alocarea acestora in camere.

### Funcționalități Cheie
- *Autentificare și Autorizare*:
  - Permite autentificarea utilizatorilor (studenți și administratori) și aplicarea de roluri și permisiuni adecvate.
- *Gestionarea documentelor necesare pentru cazare*:
  - Studenții pot încărca documentele pe platformă, iar administratorii pot aproba sau respinge documentele acestora în cazul în care acestea sunt bune sau nu.
- *Alocare Automată a Camerelor*:
  - Alocarea bazată pe criterii predefinite, asigurând o distribuție echitabilă a camerelor.
- *Panou de Administrare*:
  - Interfață pentru administratori pentru gestionarea utilizatorilor, camerelor și documentelor studenților.
- *Sistem de notificări și anunțuri*:
  - Secțiune de notificări și anunțuri pentru studenți și admini unde studenții pot vizualiza notificările și anunțurile trimise/postate de admini.
  - Adminii pot trimite notificări către fiecare student în parte și posta anunțuri pentru ca acestea să fie vizualizate de toți studenții.

### Tehnologii Utilizate
- *Backend*: ASP.NET Core
- *ORM*: Entity Framework Core
- *Bază de date*: PostgreSQL Server
- *Deploy*: Container de Docker pe care se rulează baza de date

