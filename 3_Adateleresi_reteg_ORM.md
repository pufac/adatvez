Természetesen, végigvezetlek a diasoron, és megpróbálom a lehető legérthetőbben elmagyarázni az egyes fogalmakat. Ez egy nagyon fontos és alapvető tárgy a szoftverfejlesztésben. Vágjunk is bele!

### Bevezetés: Mik azok az adatvezérelt rendszerek? (1-6. dia)

**1. dia: Címlap**
Ez a bemutatkozó dia. A tárgy neve "Adatvezérelt rendszerek", az előadó Albert István a BME Automatizálási és Alkalmazott Informatikai Tanszékéről (AUT).

**2-4. dia: Mi az az adatvezérelt rendszer?**
Ahelyett, hogy egy bonyolult definícióval indítana, az előadás példákkal mutatja be, miről is van szó. Az adatvezérelt rendszerek olyan szoftverek, amelyeknek a központi funkciója az adatok tárolása, kezelése, módosítása és megjelenítése a felhasználók számára.

*   **Példák (4. dia):** Facebook, Twitter, Stack Overflow, Booking.com, a NEPTUN (egyetemi tanulmányi rendszer), vagy az EESZT (Elektronikus Egészségügyi Szolgáltatási Tér). Mindegyikre igaz, hogy a lényege az adatok köré épül: posztok, üzenetek, kérdések-válaszok, foglalások, jegyek, egészségügyi adatok.

**5. dia: Példa – Gmail**
A Gmail egy tökéletes példa. Gondolj bele, mi minden történik adatszinten:
*   **Weboldal, mobil alkalmazás:** Ezek a felületek, ahol látod az adatokat (az e-maileket).
*   **POP3 protokoll:** Egy másik csatorna az adatok elérésére.
*   **Funkciók:** Üzenetek küldése, fogadása, csillagozás, kategorizálás, csatolmányok kezelése. Mindegyik egy-egy adatkezelési művelet. A rendszer magja egy óriási adatbázis, ami az e-maileket és a hozzájuk tartozó információkat tárolja.

**6. dia: Mi NEM szkópja ennek a tárgynak?**
Ez a dia segít megérteni a határokat. Bár ezek a rendszerek is használnak adatokat, a fókuszuk máshol van:
*   **Folyamatautomatizálás (fogaskerekek):** Itt a cél egy ipari vagy üzleti folyamat vezérlése.
*   **OpenAI/ChatGPT:** Ez egy mesterséges intelligencia modell, a hangsúly a nyelvi feldolgozáson és a generáláson van, nem a strukturált adatbázis-kezelésen.
*   **Önvezető autó, repülőgép-szimulátor:** Ezek valós idejű, rendkívül komplex rendszerek, ahol a fizikai modellezés és a gyors reakcióidő a lényeg.

**Összefoglalva:** Az adatvezérelt rendszerek azok a szoftverek, ahol a "biznisz" az adatok hatékony és megbízható kezelése.

---

### Architektúrák: Hogyan épülnek fel ezek a rendszerek? (7-12. dia)

**7-9. dia: Háromrétegű architektúra**
Ez a modern szoftverfejlesztés egyik legfontosabb alapelve. Ahelyett, hogy egyetlen óriási programot írnánk, a feladatokat logikailag szétválasztjuk három fő rétegre:

1.  **Bemutató réteg (Presentation Layer):** Ez az, amit a felhasználó lát. A felhasználói felület (UI). Feladata, hogy megjelenítse az adatokat és fogadja a felhasználói interakciókat (kattintások, gépelés). Pl. egy weboldal vagy egy mobilalkalmazás képernyője.
2.  **Üzleti logikai réteg (Business Layer):** Ez a rendszer "agya". Itt történnek a számítások, az adatok feldolgozása, és itt vannak a szabályok. Például amikor a webshopban a kosárba teszel valamit, ez a réteg számolja ki a végösszeget, ellenőrzi a raktárkészletet stb. Ez a réteg nem tudja, hogy az adatokat egy weboldal vagy egy mobilapp mutatja-e meg.
3.  **Adat réteg (Data Layer):** Ennek a rétegnek egyetlen feladata van: az adatok fizikai tárolása és elérése (írás, olvasás, törlés). Jellemzően egy adatbáziskezelő rendszer (pl. MS SQL Server) tartozik ide.

**Fontos különbség (9. dia):**
*   **Logikai réteg (layer):** A kód felelősségi körök szerinti szétválasztása. Ez segít a fejlesztőknek, hogy átláthatóbb, karbantarthatóbb kódot írjanak.
*   **Fizikai réteg (tier):** A programrészek fizikai szétválasztása, azaz melyik réteg melyik számítógépen (szerveren) fut. A háromrétegű architektúra gyakran három fizikai rétegen valósul meg (pl. a böngésződ a kliens, a webszerver futtatja az üzleti logikát, és egy külön adatbázisszerver tárolja az adatokat).

**10. dia: Gmail példa a háromrétegű architektúrára**
*   **Bemutató réteg:** A HTML/CSS kód, amit a böngésződ megjelenít, vagy az Android komponensek a telefonodon.
*   **Üzleti logikai réteg:** Azok a funkciók, mint az üzenetek keresése, a csillagozás logikája, a késleltetett e-mail küldés. Ezeket egy REST interfészen keresztül éri el a bemutató réteg.
*   **Adat réteg:** Maga az adatbázis, ahol az üzenetek és a csatolmányok (pl. Google Drive-on) vannak tárolva.

**11. dia: Két- vs. háromrétegű alkalmazás**
A régebbi, ún. két rétegű ("kliens-szerver") modellekben az üzleti logika vagy a kliens gépén (a UI-ban), vagy az adatbázisban (tárolt eljárások formájában) volt. Ez nem volt rugalmas, nehéz volt skálázni (sok felhasználót kiszolgálni) és biztonsági problémákat vetett fel. A három réteg ezt oldja meg.

**12. dia: Mikroszolgáltatás architektúra**
Ez a háromrétegű modell egy modernebb továbbfejlesztése. Ahelyett, hogy egyetlen nagy "üzleti logikai réteg" lenne, ezt a réteget kisebb, önálló, független szolgáltatásokra bontják (pl. Felhasználókezelő szolgáltatás, Termékkatalógus szolgáltatás, Rendeléskezelő szolgáltatás). Mindegyiknek lehet akár saját, külön adatbázisa is. Előnye, hogy rugalmasabb, könnyebben fejleszthető és skálázható.

---

### Félév áttekintése: Miről lesz szó? (13-18. dia)

Ez a rész bemutatja, hogy a félév során mely technológiákkal fogtok foglalkozni az egyes rétegekben.

**14. dia: Adatréteg technológiái**
*   **MS SQL Server:** Egy klasszikus relációs adatbáziskezelő.
*   **MongoDB:** Egy NoSQL adatbázis, ami nem táblázatokban, hanem dokumentumokban (JSON-szerűen) tárolja az adatokat.
*   **Párhuzamos adathozzáférés:** Hogyan kezeljük, ha sokan egyszerre akarják ugyanazt az adatot elérni (erről szólnak a tranzakciók).
*   **Lekérdezés optimalizálás:** Hogyan lehet a lekérdezéseket gyorssá tenni.

**15. dia: Adatelérési és üzleti logikai réteg technológiái**
*   **O/R leképezés (ORM - Object-Relational Mapping):** Olyan eszközök (pl. Entity Framework, JPA), amik "lefordítják" a programozási nyelv objektumait az adatbázis tábláira, így a fejlesztőnek nem kell közvetlenül SQL-t írnia.
*   **SOAP, REST, GraphQL:** Különböző technológiák arra, hogy a rétegek (pl. a bemutató és az üzleti) hogyan kommunikáljanak egymással a hálózaton keresztül (ezek az ún. API-k).

**16-18. dia: Gyakorlatok és a minta adatbázis**
A félév során egy játékbolt adatbázisával fogtok dolgozni. A 18. dián látható az adatmodell (Entity-Relationship Diagram), ami bemutatja a táblákat (pl. `Customer`, `Order`, `Product`) és a köztük lévő kapcsolatokat.

---

### Tranzakciók és az ACID-alapelvek (19-28. dia)

Ez a bemutató egyik legfontosabb, elméleti része.

**19-22. dia: A probléma: Konkurens adathozzáférés**
Mi történik, ha több felhasználó pontosan ugyanabban a pillanatban próbálja módosítani ugyanazt az adatot? Például ketten próbálják megvenni az utolsó repülőjegyet. Ha a rendszer ezt nem kezeli le jól, káosz alakul ki: mindketten azt hiszik, megvették, vagy az adatbázis inkonzisztens állapotba kerül. A kutyás képek ezt a "káoszt" szimbolizálják.

**23. dia: Tranzakció fogalma és az ACID**
A **tranzakció** egy vagy több adatbázis-művelet (pl. olvasás, írás) sorozata, amit egyetlen logikai egységként kezelünk. A klasszikus példa a banki átutalás: ez két műveletből áll (egyik számláról levonni, a másikra jóváírni).

Egy tranzakciónak négy alapvető tulajdonsággal kell rendelkeznie. Ezek az **ACID** betűszóval írhatók le:

*   **A - Atomicity (Atomiság):** "Mindent vagy semmit". A tranzakció vagy teljesen lefut (minden művelete sikeres), vagy ha bármi hiba történik, akkor teljesen visszaáll az eredeti állapot, mintha meg sem történt volna. Az átutalásnál nem fordulhat elő, hogy az egyik számláról már levontuk a pénzt, de a másikra még nem érkezett meg.
*   **C - Consistency (Konzisztencia):** A tranzakció az adatbázist csak egyik konzisztens (érvényes) állapotból egy másik konzisztens állapotba viheti át. A bankos példánál maradva: a rendszer szabálya, hogy a pénz nem veszhet el. A tranzakció előtt és után is ugyanannyi a teljes pénzmennyiség a bankban.
*   **I - Isolation (Izoláció):** Az egyszerre futó tranzakciók nem láthatnak egymás "félkész" állapotaira. Úgy kell viselkedniük, mintha egymás után futnának le, sorban. Így nem zavarhatják meg egymást.
*   **D - Durability (Tartósság):** Ha egy tranzakció sikeresen lefutott (ezt `COMMIT`-nak hívjuk), akkor az általa végzett változtatások véglegesek, és egy esetleges rendszerhiba (pl. áramszünet) után is megmaradnak.

**24-28. dia:** Ezek a diák vizuálisan mutatják be az ACID elveket, főleg az atomiságot és az izolációt. A tartósságot a **tranzakciós napló (transaction log)** biztosítja, ami egyfajta "biztonsági napló" a változtatásokról.

---

### Tranzakciós izolációs szintek (29-50. dia)

Ez a legbonyolultabb rész. Az "izoláció" a gyakorlatban nem fekete-fehér. A tökéletes izoláció (mintha mindenki egyedül használná a rendszert) nagyon lassú lenne. Ezért léteznek különböző **izolációs szintek**, amik kompromisszumot jelentenek a sebesség és a biztonság között.

**31. dia: Párhuzamos futtatás tipikus problémái (anomáliák):**
*   **Piszkos olvasás (Dirty Read):** Egy tranzakció kiolvas egy olyan adatot, amit egy másik tranzakció már módosított, de még nem véglegesített (`COMMIT`-ált). Ha a második tranzakció végül visszavonja a módosítást (`ROLLBACK`), az első tranzakció érvénytelen ("piszkos") adattal dolgozott tovább.
*   **Nem megismételhető olvasás (Non-Repeatable Read):** Egy tranzakció kiolvas egy adatot. Mielőtt újra kiolvasná, egy másik tranzakció módosítja azt. Amikor az első tranzakció újra olvas, már más értéket kap.
*   **Fantom rekordok (Phantom Read):** Egy tranzakció lekérdez egy sorhalmazt (pl. "add ide az összes budapesti ügyfelet"). Mielőtt újra lefuttatná ugyanezt a lekérdezést, egy másik tranzakció beszúr egy új, a feltételnek megfelelő sort (egy új budapesti ügyfelet). Amikor az első tranzakció újra lekérdez, egy "fantom" sort is lát, ami korábban nem volt ott.
*   **Elveszett módosítás (Lost Update):** Két tranzakció beolvassa ugyanazt az értéket (pl. 10). Mindkettő hozzáad 1-et, és visszaírja az eredményt (11). A végeredmény 11 lesz 12 helyett, az egyik módosítás elveszett.

**32. és 44. dia: SQL ANSI szabványos izolációs szintek:**
Ezek a szintek határozzák meg, hogy a fenti anomáliák közül melyiket engedik meg.
1.  **Read Uncommitted:** A leggyengébb szint, mindent megenged. Nagyon gyors, de veszélyes.
2.  **Read Committed:** Megakadályozza a piszkos olvasást. Ez a legtöbb adatbázis alapértelmezett beállítása.
3.  **Repeatable Read:** Megakadályozza a piszkos és a nem megismételhető olvasást is.
4.  **Serializable:** A legerősebb szint, mindent megakadályoz. Olyan, mintha a tranzakciók ténylegesen egymás után futnának, de ez a leglassabb.

**Hogyan működik? Zárakkal (Locks) (33. dia):**
Az adatbáziskezelő "zárakat" helyez el az adatokon.
*   **Shared lock (megosztott zár):** Több tranzakció is olvashatja ugyanazt az adatot egyszerre.
*   **Exclusive lock (kizárólagos zár):** Ha egy tranzakció írni akar egy adatot, kizárólagos zárat tesz rá, és amíg ezt fel nem oldja, senki más nem férhet hozzá (sem írni, sem olvasni).
Az izolációs szintek abban különböznek, hogy milyen zárakat és mennyi ideig használnak.

**47-49. dia: Snapshot Isolation (Pillanatkép izoláció)**
Ez egy modernebb, zárak nélküli megoldás. Ahelyett, hogy zárolná az adatokat, a rendszer minden tranzakciónak egy "pillanatképet" ad az adatbázisról, ami a tranzakció indulásának pillanatában volt érvényes. A tranzakció ezen a verzión dolgozik, és `COMMIT` idején a rendszer ellenőrzi, hogy közben más módosította-e ugyanazt az adatot. Ha igen, az egyik tranzakciót visszagörgeti. Ezt **optimista** konkurrenciakezelésnek hívják (reméljük a legjobbakat, és a végén ellenőrzünk), szemben a zárolás **pesszimista** megközelítésével (eleve lezárjuk az adatot, mert a legrosszabbra számítunk).

---

### Tranzakciós napló és helyreállítás (51-63. dia)

Ez a rész a **Durability** (Tartósság) megvalósításáról szól.

Az adatbázis a gyorsaság érdekében a memóriában (RAM) dolgozik, és csak időnként írja ki a változásokat a lemezre (merevlemez/SSD). De mi van, ha a kiírás előtt elmegy az áram? Ekkor jön a képbe a **tranzakciós napló (log)**.

Minden módosítást, ami történik, az adatbázis azonnal beleír a naplófájlba a lemezen. Ez a naplóírás nagyon gyors. Ha a rendszer összeomlik, újraindítás után az adatbáziskezelő megnézi a naplót, és helyreállítja a konzisztens állapotot:
*   A `COMMIT`-ált, de lemezre még ki nem írt tranzakciók változtatásait a napló alapján újra végrehajtja (**REDO**).
*   Az el sem kezdett vagy be nem fejezett (`ROLLBACK`-elt) tranzakciók változtatásait a napló alapján visszavonja (**UNDO**).

A diák különböző naplózási stratégiákat mutatnak be (Undo, Redo, Undo/Redo).

---

### Elosztott rendszerek (64-73. dia)

Mi van, ha az adatbázisunk olyan óriási, hogy nem fér el egyetlen szerveren, vagy a felhasználóink a világ különböző pontjain vannak? Ekkor elosztott rendszerekről beszélünk.

**65. dia: Elosztott tranzakciók**
Amikor egy tranzakció több adatbázis-szervert is érint. Ezt nagyon bonyolult atomikusan (mindent vagy semmit alapon) végrehajtani, speciális protokollok (pl. kétfázisú commit) kellenek hozzá.

**67. dia: CAP-tétel**
Ez az elosztott rendszerek egyik legfontosabb elméleti tétele. Azt mondja ki, hogy egy elosztott rendszer a következő három tulajdonság közül egyszerre legfeljebb kettővel rendelkezhet:

1.  **C - Consistency (Konzisztencia):** Mindenki ugyanazt az adatot látja minden időpillanatban.
2.  **A - Availability (Rendelkezésre állás):** A rendszer mindig válaszol a kérésekre (nem ad hibát).
3.  **P - Partition Tolerance (Partícionálástűrés):** A rendszer akkor is működőképes marad, ha a hálózati kapcsolat megszakad a szerverek (csomópontok) között.

Mivel hálózati hibák (partíciók) mindig előfordulhatnak az interneten, a `P`-t muszáj biztosítani. Tehát a valódi választás a `C` és az `A` között van:
*   **CP rendszer (Konzisztencia > Rendelkezésre állás):** Ha megszakad a kapcsolat, a rendszer inkább leáll (nem válaszol), de nem ad ki elavult adatot. (Pl. banki rendszerek)
*   **AP rendszer (Rendelkezésre állás > Konzisztencia):** Ha megszakad a kapcsolat, a rendszer akkor is válaszol, legfeljebb egy kicsit elavult adattal. (Pl. a Facebook üzenőfalad).

**69. dia: Strong vs. Eventual Consistency**
*   **Strong Consistency (Erős konzisztencia):** Ez a `CP` rendszerek tulajdonsága. Az írás után minden olvasás már az új adatot adja vissza.
*   **Eventual Consistency (Végül is konzisztencia):** Ez az `AP` rendszereké. Az írás után egy darabig még előfordulhat, hogy egyes felhasználók a régi adatot látják, de a rendszer garantálja, hogy "végül" mindenki megkapja a frissítést.

Remélem, ez a részletes magyarázat segít megérteni a diasor tartalmát!

---

### Ellenőrző kérdések

Hogy lásd, mennyire sikerült megérteni a kulcsfogalmakat, próbáld megválaszolni ezeket a kérdéseket a saját szavaiddal:

1.  Mi a háromrétegű architektúra lényege? Mi a különbség a Bemutató és az Üzleti logikai réteg között egy webshop példáján keresztül?
2.  Mi az az ACID? Miért fontos az "Atomiság" egy banki átutalásnál?
3.  Mi a különbség a "Nem megismételhető olvasás" és a "Fantom rekord" anomáliák között? Melyik izolációs szint védi ki ezeket?
4.  Mire jó a CAP-tétel? Miért nem lehet egyszerre egy rendszer konzisztens, mindig elérhető és partíciótűrő?

Ha bármelyik kérdés nehéznek tűnik, vagy van még valami, ami nem tiszta, kérdezz bátran