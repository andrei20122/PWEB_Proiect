# PWEB_Proiect
Am dezvoltat în ASP.NET Core un server pentru o aplicație web care gestionează cazarea studenților în cămine.
Sistemul oferă funcționalități complete de la aplicarea pentru locuri de cazare până la gestionarea documentelor studentilor si alocarea acestora in camere.

Funcționalități Cheie
Autentificare și Autorizare:
Permite autentificarea utilizatorilor (studenți și administratori) și aplicarea de roluri și permisiuni adecvate.
Gestionarea documentelor necesare pentru cazare:
Studenții pot incarca documentele pe platforma, iar administratorii pot aproba sau respinge documentele acestora in cazul in care aceestea sunt bune sau nu.
Alocarea Automată a Camerelor:
Alocare bazată pe criterii predefinite, asigurând o distribuție echitabilă a camerelor.
Panou de Administrare:
Interfață pentru administratori pentru a gestiona utilizatorii, camerele și documentele studentilor.
Sectiune de notificari si anunturi:
Sectiune de notificari si anunturi pentru studenti si admini unde studentii pot vizualiza notificarile si anunturile trimise/postate de admini.
Adminii pot trimite notificarti catre fiecare student in parte si posta anunturi pentru ca acestea sa fie vizualizate de toti studentii.

Tehnologii Utilizate
Backend: ASP.NET Core
ORM: Entity Framework Core
Baza de Date: PostgreSQL Server
Deploy: Container de Docker pe care sa ruleze baza de date
