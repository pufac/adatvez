Rendben, vágjunk is bele a második diasorba! Ez a prezentáció sokkal gyakorlatiasabb, és a Microsoft SQL Server (MSSQL) specifikus programozási megoldásaira fókuszál. Ahogy kérted, minden részletre kitérek, a megszokott érthető és strukturált formában.

---

### Bevezetés és Tartalom (1-3. dia)

**1. dia: Címlap**
Ez a bevezető, a téma az **"Adatvezérelt rendszerek: Microsoft SQL Server programozása"**. Ez azt jelenti, hogy most az elméleti alapok után megnézzük, hogyan lehet konkrét programkódokat írni és futtatni közvetlenül az adatbázis-szerveren belül.

**2. dia: Tartalom**
Ez a dia felvázolja az előadás szerkezetét. A főbb témakörök:
*   **MS SQL Server platform:** Általános áttekintés a rendszerről.
*   **Szerver oldali programozás:** Miért és hogyan programozzuk az adatbázist magát?
*   **Transact-SQL (T-SQL) nyelv:** Ez az SQL nyelv Microsoft által kibővített változata, amivel programozni lehet. Itt a fő elemek:
    *   **Kurzorok:** Soronkénti adatfeldolgozás.
    *   **Tárolt eljárások és függvények:** Újrafelhasználható kódblokkok.
    *   **Hibakezelés:** Hogyan tegyük robusztussá a kódunkat.
    *   **Triggerek:** Adatmódosításokra automatikusan lefutó programok.

**3. dia:** Ez csupán egy elválasztó dia a következő témakörhöz.

---

### Adatbázis-elméleti alapok ismétlése (4-6. dia)

Ez a rész egy gyors ismétlés, hogy mindenki ugyanazokról a fogalmakról beszéljen.

**4. dia: Adatbázis definíció**
*   **Definíció:** Logikailag összefüggő, rendezett adatgyűjtemény.
    *   **Összefüggő:** Az adatok egy adott alkalmazáshoz vagy problémához kapcsolódnak (pl. egy webshop adatai).
    *   **Rendezett:** Az adatok struktúráltak, így könnyen lehet bennük keresni, szűrni.
*   **Adat:** Bármilyen rögzíthető tény. A dia három kategóriát említ:
    *   **Hagyományos:** Szöveg, szám, dátum.
    *   **Multimédia:** Kép, hang, videó.
    *   **Strukturált:** XML vagy JSON, amik önmagukban is összetett adatstruktúrákat írnak le.

**5. dia: Relációk (táblák) tulajdonságai**
Ez a relációs adatbázisok alapja.
*   A tábla sorokból és oszlopokból áll.
*   **Rekord / sor:** A tábla egy sora, ami egy konkrét entitást ír le (pl. egy felhasználó adatai).
*   **Oszlop / attribútum:** A tábla egy oszlopa, ami egy entitás egy tulajdonságát írja le (pl. a felhasználó neve).
*   **Skalár érték:** A tábla egy cellájában mindig csak egyetlen, oszthatatlan (atomikus) érték lehet. Nem lehet egy cellában egy lista.

**6. dia: Integritási kritériumok**
Ezek azok a szabályok, amik biztosítják az adatok helyességét és konzisztenciáját.
1.  **Entitás integritás:** `kulcs ≠ NULL`. Minden táblának kell lennie egy elsődleges kulcsának (egy vagy több oszlop), ami egyértelműen azonosít minden sort. Ennek a kulcsnak az értéke sosem lehet üres (`NULL`).
2.  **Tartományi integritás:** Minden oszlopnak van egy adattípusa (pl. `int`, `varchar`) és lehetnek további megkötései (pl. `Életkor > 0`). Az oszlopba csak a típusnak és a megkötéseknek megfelelő értékeket lehet beírni.
3.  **Referenciális integritás:** Ez a táblák közötti kapcsolatok helyességét biztosítja a **külső kulcsok (foreign key)** segítségével. A dián a `Rek999` érték az `OszlopB`-ben egy külső kulcs, ami egy másik tábla egy sorára hivatkozik. A szabály az, hogy itt csak olyan érték szerepelhet, ami a hivatkozott táblában elsődleges kulcsként létezik. (Nem lehet pl. egy nem létező felhasználóhoz rendelést felvenni.)

---

### A Microsoft SQL Server Platform (7-11. dia)

**7. dia: MSSQL komponensek**
Az SQL Server nem csak egyetlen program, hanem egy szolgáltatáscsomag.
*   **SQL Server Service:** Maga az adatbázis motor, a rendszer lelke.
*   **SQL Server Agent:** Időzített feladatok futtatására szolgál (pl. éjszakai mentések, karbantartási scriptek).
*   **A többi (Analysis, Reporting, Integration Services):** Ezek az ún. Business Intelligence (BI) eszközök, amikkel komplex adatelemzéseket, riportokat és adatátalakítási folyamatokat lehet végezni.

**8. dia: Adatbázisok tárolása**
Egy MSSQL adatbázis fizikailag legalább két fájlból áll:
*   **Adatfájl (.mdf - Master Data File):** Ebben vannak a táblák, indexek, tehát a tényleges adatok. A nagyobb teljesítmény érdekében több fájlra és több fizikai lemezre is szétosztható (`Filegroup`).
*   **Tranzakciós napló (.ldf - Log Data File):** Ez az előző előadásban tárgyalt feketedoboz, ami a tartósságot (Durability) biztosítja. Minden módosítás először ide kerül be.
Egy adatbázison belül **sémák (schema)** segítségével lehet logikailag csoportosítani az objektumokat (táblákat, eljárásokat). Az alapértelmezett séma a `dbo` (database owner).

**9. dia: Felhasználói séma elemei**
Ezek azok az "objektumok", amiket létrehozhatsz egy adatbázisban:
*   **Tábla:** Adatok tárolására.
*   **Nézet (View):** Egy elmentett `SELECT` utasítás, ami virtuális táblaként viselkedik. Elrejtheti a bonyolult lekérdezéseket vagy korlátozhatja a látható adatokat.
*   **Index:** A táblák adatai feletti segéd-adatstruktúra, ami a lekérdezéseket gyorsítja (mint egy könyv tárgymutatója).
*   **Szekvencia (Sequence):** Egy számláló, ami táblától függetlenül tud egyedi számokat generálni.
*   **Programmodul:** Ezek a szerver oldali programozás építőkövei: **Eljárás (Procedure)**, **Függvény (Function)**, **Trigger**.

**10. dia: Legfontosabb adattípusok**
*   **Szöveges:**
    *   `char(n)`: Fix hosszúságú szöveg (mindig `n` karaktert foglal).
    *   `varchar(n)`: Változó hosszúságú szöveg (maximum `n` karaktert foglal).
    *   `nchar`, `nvarchar`: Ugyanezek, de Unicode karaktereket is támogatnak (pl. ékezetes, cirill betűk), ezért dupla annyi helyet foglalnak.
*   **Numerikus:** `int` (egész szám), `float` (lebegőpontos), `numeric` (fixpontos, pénzügyi számításokhoz ideális).
*   **Dátum:** `datetime`, `datetime2` (utóbbi pontosabb).

**11. dia: Kitérő: Hol tároljuk a képeket?**
Ez egy klasszikus dilemma. Három fő lehetőség van, mindnek van előnye és hátránya:
1.  **Adatbázisban:** Előny: tranzakcionálisan kezelhető, a mentéssel együtt mentődik. Hátrány: lassú, terheli az adatbázist, megnöveli a méretét.
2.  **Fájlrendszerben:** Előny: gyors. Hátrány: nem tranzakcionális (ha a fájl törlése sikerül, de az adatbázis-rekordé nem, inkonzisztencia lép fel), a mentést külön kell megoldani.
3.  **Külső/felhő szolgáltatásban (pl. Amazon S3):** Manapság ez a leggyakoribb. Előny: skálázható, megbízható, nem terheli a saját rendszerünket. Hátrány: külső függőség, költséges lehet.

---

### Szerveroldali Programozás: A Transact-SQL (T-SQL) Nyelv

**12-15. dia: Elsődleges kulcsok és a Scope vs. Session probléma**
*   **`identity(1,1)`:** Ezzel a kulcsszóval automatikusan növekvő (1-től 1-esével) egész szám típusú elsődleges kulcsot hozhatunk létre.
*   **Probléma:** Miután beszúrtunk egy új sort, gyakran szükségünk van a frissen generált ID-ra. Hogyan kérdezzük le?
    *   `@@IDENTITY`: Visszaadja a **kapcsolaton (session)** belül legutoljára generált identity értéket. **VESZÉLYES!** Ha a beszúrásunk elindít egy másik táblán egy triggert, ami szintén beszúr egy identity-s sort, akkor az `@@IDENTITY` a trigger által beszúrt ID-t fogja visszaadni, nem a miénket!
    *   `SCOPE_IDENTITY()`: Visszaadja az aktuális **hatókörön (scope)** – azaz a mi futó kódblokkunkon – belül generált utolsó identity értéket. **EZT HASZNÁLD!** Ez nem veszi figyelembe a triggerek által generált ID-kat.
    *   `IDENT_CURRENT('táblanév')`: Visszaadja a megadott táblába legutoljára generált ID-t, függetlenül a sessiontől és a scope-tól.

**16-18. dia: Tranzakciós határok és hozzáférés**
*   Az MSSQL alapból `Auto commit` módban működik: minden egyes `INSERT`, `UPDATE`, `DELETE` utasítás egy külön, önálló tranzakció. Ha több utasítást akarunk egy tranzakcióba foglalni, azt `BEGIN TRANSACTION` ... `COMMIT TRANSACTION` közé kell tenni.
*   Az izolációs szintek (az előző előadásból) itt is érvényesek. Az alapértelmezett a `Read committed`. Az MSSQL támogat egy nem szabványos, de nagyon hatékony szintet is, a `Snapshot`-ot.
*   A hozzáférést nagyon részletesen lehet szabályozni: teljes szerver, egy adatbázis, egy séma, egy tábla, vagy akár egyetlen oszlop szintjén is lehet jogokat adni (`GRANT`) vagy tiltani (`DENY`).

**19-26. dia: A szerveroldali programozás motivációja**
*   **Miért ne a kliens oldalon (pl. C# vagy Java kódban) oldjuk meg?** A Neptun vizsgajelentkezéses példa tökéletesen bemutatja: Ha két diák egyszerre jelentkezik az utolsó helyre, a kliens oldali logika hibázhat. Mindkét kliens lekérdezheti, hogy "van még 1 hely", mindkettő megpróbálja beszúrni a jelentkezést, és versenyhelyzet (race condition) alakul ki.
*   **Megoldás: Tárolt eljárás (Stored Procedure)**
    *   A teljes logika (1. Helyek számának ellenőrzése, 2. Jelentkezés beszúrása) bekerül egyetlen, az adatbázis-szerveren futó programba.
    *   A kliens már csak ezt az egy eljárást hívja meg.
    *   Mivel ez a logika egy tranzakción belül, atomi egységként fut le a szerveren, a versenyhelyzet kizárt.
*   **Előnyök:** Konzisztencia biztosítása, nagyobb biztonság (a kliens csak az eljárást hívhatja, a táblákat közvetlenül nem módosíthatja), jobb teljesítmény (kevesebb hálózati forgalom).
*   **Hátrányok:** A T-SQL egy régi, nem mindenhol szabványos nyelv, a szerver terhelését növeli, és nehezebb skálázni, mint egy külön alkalmazásszervert.

**27-39. dia: A T-SQL nyelv és a Kurzorok**
*   **T-SQL:** Ez a szabványos SQL, kiegészítve programozási elemekkel: változók (`DECLARE @nev ...`), vezérlési szerkezetek (`IF-ELSE`, `WHILE`), blokkok (`BEGIN-END`).
*   **Kurzorok:** Az SQL alapvetően halmaz-alapú. Egy `UPDATE` egyszerre akár ezer sort is módosíthat. A kurzor ennek az ellentéte: egy `SELECT` eredményhalmazán teszi lehetővé a **soronkénti végiglépkedést**, mint egy `foreach` ciklus.
    *   **Mikor használjuk?** Csak akkor, ha halmaz-alapú megoldás nincs, vagy rendkívül bonyolult lenne.
    *   **Használata:** `DECLARE` (definiálás), `OPEN` (lekérdezés futtatása), `FETCH NEXT INTO @valtozo` (következő sor adatainak beolvasása változókba), `WHILE @@FETCH_STATUS = 0` (ciklus, amíg van sor), `CLOSE` (bezárás), `DEALLOCATE` (memória felszabadítása).
    *   **FIGYELEM:** A kurzorok általában lassúak és erőforrás-igényesek. Ha lehet, kerülni kell őket!

**40-46. dia: Tárolt eljárások és függvények**
*   **Tárolt eljárás (Stored Procedure):** Paraméterezhető, újrafelhasználható T-SQL kódblokk. Bármit csinálhat: adatot módosíthat, kérdezhet le, tranzakciót kezelhet.
*   **Függvény (Function):** Olyan, mint egy eljárás, de van egy fontos megkötése: **van visszatérési értéke, és nem módosíthatja az adatbázis állapotát (csak olvashat).**
    *   **Scalar-valued:** Egyetlen értéket ad vissza (pl. egy számot, szöveget).
    *   **Table-valued:** Egy teljes táblát (rekordhalmazt) ad vissza, amit utána egy `SELECT` utasítás `FROM` részében fel lehet használni.

**47-52. dia: Hibakezelés**
*   **Régi módszer (`@@ERROR`):** Minden egyes utasítás után manuálisan ellenőrizni kellett az `@@ERROR` globális változó értékét. Ha 0, nem volt hiba. Ez körülményes és könnyen elfelejthető.
*   **Modern módszer (`TRY...CATCH`):** A legtöbb modern programozási nyelvhez hasonlóan itt is van strukturált kivételkezelés.
    *   A `BEGIN TRY ... END TRY` blokkba kerül a "védett" kód.
    *   Ha itt hiba történik, a vezérlés azonnal a `BEGIN CATCH ... END CATCH` blokkba ugrik.
    *   A `CATCH` blokkban le lehet kérdezni a hiba részleteit (`ERROR_NUMBER()`, `ERROR_MESSAGE()`, stb.), és lehet rá reagálni: naplózni, vagy a `THROW` paranccsal továbbdobni a hibát a hívónak.
    *   A `THROW` paranccsal saját, egyedi hibákat is dobhatunk.

**53-60. dia: Triggerek**
*   **Mi az a trigger?** Egy speciális tárolt eljárás, ami nem manuálisan kerül meghívásra, hanem **automatikusan lefut**, amikor egy táblán egy adott esemény (`INSERT`, `UPDATE` vagy `DELETE`) bekövetkezik.
*   **Mire jó?**
    *   **Naplózás (Audit):** Ha egy rekordot törölnek, a trigger automatikusan beírhatja egy naplótáblába, hogy ki és mikor törölte.
    *   **Származtatott adatok karbantartása:** Ha egy rendeléshez új tételt vesznek fel, egy trigger automatikusan frissítheti a rendelés végösszegét tartalmazó oszlopot.
    *   **Komplex szabályok:** Olyan üzleti szabályok kikényszerítése, amiket egyszerű `CHECK` megkötéssel nem lehet.
*   **Hogyan működik?** A triggeren belül két speciális, csak olvasható, memóriában létező tábla érhető el:
    *   `inserted`: Tartalmazza az `INSERT` vagy `UPDATE` művelet utáni új sor(oka)t.
    *   `deleted`: Tartalmazza a `DELETE` vagy `UPDATE` művelet előtti régi sor(oka)t.
    *   **FONTOS:** A trigger **utasításonként egyszer fut le**, nem soronként! Ha egy `DELETE` 100 sort töröl, a trigger egyszer fut le, és a `deleted` táblában 100 sor lesz. A trigger kódjának fel kell készülnie több sor kezelésére.

**61-62. dia: Érdekesség: batch**
*   **Mi az a batch?** Az a T-SQL utasításhalmaz, amit a kliens (pl. a Management Studio) egyszerre küld fel végrehajtásra a szervernek.
*   **`GO` kulcsszó:** Ez **NEM T-SQL parancs!** Ez csak egy elválasztó jel a kliens eszköz számára, ami azt jelzi, hogy "eddig tartott egy batch, küldd el a szervernek, majd folytasd a következővel". Ezért van az, hogy egy változót nem lehet deklarálni egy batch-ben és használni egy másikban.

Remélem, ez a részletes magyarázat segített tisztázni a Microsoft SQL Server programozásának alapjait! Ha van még kérdésed, tedd fel bátran