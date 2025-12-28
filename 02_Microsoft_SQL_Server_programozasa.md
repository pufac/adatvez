Természetesen. Készítettem egy még részletesebb, minden apró részletre és kódra kiterjedő magyarázatot a második diasorhoz. Minden kódrészletet elláttam magyar nyelvű kommentekkel, hogy lépésről lépésre követhető legyen a logika.

---

### Bevezetés és Tartalom (1-3. dia)

**1. dia: Címlap**
Az előadás címe **"Adatvezérelt rendszerek: Microsoft SQL Server programozása"**. Ez a diasor az adatbázis-kezelés gyakorlati oldalára fókuszál, konkrétan arra, hogyan lehet programlogikát futtatni közvetlenül az adatbázis-szerveren belül a Microsoft SQL Server (MSSQL) rendszerben.

**2. dia: Tartalom**
Ez a dia felvázolja az előadás teljes szerkezetét. A tárgyalt témakörök:

*   **Microsoft SQL Server platform:** Általános bemutató a rendszerről és komponenseiről.
*   **Adatbázisok szerver oldali programozása:** Mi a motiváció a szerveren futó kód írására, és mik az előnyei/hátrányai.
*   **Transact-SQL (T-SQL) nyelv:** Az SQL Microsoft-specifikus, programozási elemekkel kibővített nyelve. Ennek részei:
    *   **Kurzorok:** Lehetővé teszik a lekérdezések eredményhalmazának soronkénti feldolgozását.
    *   **Tárolt eljárások és függvények:** Elmentett, újrafelhasználható kódblokkok, amelyek az adatbázisban tárolódnak.
    *   **Hibakezelés:** Technikák a programkód robusztussá tételére, hibák elkapására és kezelésére.
    *   **Triggerek:** Olyan speciális eljárások, amelyek adatbázis-eseményekre (pl. egy sor törlése) automatikusan lefutnak.

**3. dia:** Elválasztó dia a következő fejezethez.

---

### Adatbázis-elméleti alapok ismétlése (4-6. dia)

**4. dia: Adatbázis definíció**
Gyors ismétlés az alapfogalmakról:
*   **Definíció:** Az adatbázis **logikailag összefüggő adatok rendezett gyűjteménye**.
    *   *Logikailag összefüggő:* Az adatok egy adott célhoz kapcsolódnak (pl. egy cég összes ügyfél-, termék- és rendelési adata).
    *   *Rendezett:* Az adatok struktúráltak, ami lehetővé teszi a hatékony keresést.
*   **Adat:** Bármilyen rögzíthető tény, ami lehet:
    *   *Hagyományos:* Szöveg (`'alma'`), szám (`123`), dátum (`'2025-11-08'`).
    *   *Multimédia:* Kép, hang, videó (ezeket ritkán tároljuk közvetlenül az adatbázisban, ahogy azt a későbbi "Kitérő" dia tárgyalja).
    *   *Strukturált:* Olyan formátumok, mint az XML vagy JSON, amelyek komplex, beágyazott adatokat is leírhatnak.

**5. dia: Relációk (táblák) tulajdonságai**
A relációs adatbázisok alapegysége a tábla (vagy matematikailag reláció).
*   **Rekord / sor:** A tábla egy sora, ami egy adott dolog (entitás) egy példányát írja le. Például a `Felhasználók` tábla egy sora egy konkrét felhasználó.
*   **Oszlop / attribútum:** A tábla egy oszlopa, ami az entitás egy tulajdonságát írja le. Például a `Felhasználók` tábla `Név` oszlopa.
*   **Skalár érték:** Egy tábla egy cellájában (egy sor és egy oszlop metszetében) mindig egyetlen, oszthatatlan (atomikus) értéknek kell lennie. Nem tárolhatsz egy cellában egy listát, pl. `['piros', 'kék']`.

**6. dia: Integritási kritériumok**
Ezek a szabályok biztosítják, hogy az adatbázis adatai mindig helyesek és megbízhatóak legyenek.
1.  **Entitás integritás (`kulcs ≠ NULL`):** Minden sornak egyedileg azonosíthatónak kell lennie az elsődleges kulcs segítségével, ami soha nem lehet üres (`NULL`).
2.  **Tartományi integritás:** Minden oszlopba csak az oszlop adattípusának és a rá definiált egyéb szabályoknak (megkötéseknek, pl. `CHECK (Ár > 0)`) megfelelő érték kerülhet.
3.  **Referenciális integritás (külső kulcs):** Biztosítja a táblák közötti kapcsolatok érvényességét. Ha egy tábla egyik oszlopa (külső kulcs) egy másik tábla elsődleges kulcsára hivatkozik, akkor ott csak olyan érték szerepelhet, ami a hivatkozott táblában létezik, vagy `NULL` (ha megengedett). Ez megakadályozza az "árva" rekordok létrejöttét (pl. rendelés egy nem létező vevőtől).

---

### A Microsoft SQL Server Platform (7-11. dia)

**7. dia: MSSQL komponensek**
Az SQL Server egy komplex termékcsalád, nem csak egyetlen program.
*   **SQL Server Service:** A központi adatbázismotor, ez felel az adatok tárolásáért, lekérdezéséért és a tranzakciók kezeléséért.
*   **SQL Server Agent:** Időzített feladatok (jobok) futtatására szolgáló alrendszer. Pl. éjszakai adatbázismentések, heti riportok generálása.
*   **Analysis Services (SSAS):** Komplex, többdimenziós adatelemzésre (OLAP kockák) és adatbányászatra szolgál.
*   **Reporting Services (SSRS):** Professzionális riportok, jelentések készítésére és terjesztésére használható.
*   **Integration Services (SSIS):** Adatintegrációs (ETL - Extract, Transform, Load) feladatok elvégzésére. Különböző forrásokból (pl. Excel fájlok, más adatbázisok) tud adatokat beolvasni, átalakítani és betölteni az SQL Serverbe.

**8. dia: Adatbázisok tárolása**
*   Egy MSSQL adatbázis fizikailag fájlokból áll:
    *   **Adatfájl (`.mdf`):** Itt tárolódnak a felhasználói adatok (táblák, indexek).
    *   **Tranzakciós napló (`.ldf`):** A feketedoboz, ami minden módosítást rögzít a tartósság (Durability) biztosításához.
*   **Séma (schema):** Egy adatbázison belüli név-konténer az objektumok (táblák, eljárások) logikai csoportosítására és a hozzáférés szabályozására. Alapértelmezett a `dbo`.
*   **Rendszer adatbázisok:** Ezek az SQL Server működéséhez szükségesek: `master` (a szerver összes beállítása), `model` (sablon új adatbázisokhoz), `msdb` (az Agent jobok adatai), `tempdb` (ideiglenes táblák és számítások helye).

**9. dia: Felhasználói séma elemei**
Ezeket az objektumokat hozhatod létre egy adatbázisban:
*   **Tábla, Nézet, Index:** Alapvető adatbázis-objektumok.
*   **Szekvencia:** Táblafüggetlen, megosztható számláló.
*   **Programmodul:** Ezek a szerveroldali programozás eszközei: Eljárás, Függvény, Trigger, és .NET Assembly (lehetőség van C# vagy VB.NET kódot is futtatni az adatbázison belül).

**10. dia: Legfontosabb adattípusok**
A dia bemutatja a leggyakoribb típusokat. Fontos különbségek:
*   `char` vs. `varchar`: A `char(10)` mindig 10 bájt helyet foglal, még ha csak 3 karaktert tárolsz benne is. A `varchar(10)` csak annyit foglal, amennyi szükséges (+ overhead).
*   `varchar` vs. `nvarchar`: Az `n` előtag az "National"-re utal, ami a Unicode karakterkészletet jelöli. Használd `nvarchar`-t, ha nemzetközi karaktereket (pl. 'á', 'é', 'ű') is tárolnod kell. Minden karakter 2 bájtot foglal.
*   `max`: A `varchar(max)` és `nvarchar(max)` segítségével akár 2 GB méretű szöveget is tárolhatsz.

**11. dia: Kitérő: Hol tároljuk a képeket?**
A dia egy klasszikus tervezési kérdést vet fel. A modern gyakorlat szerint a legjobb megoldás egy **külső/felhő szolgáltatás** (pl. Amazon S3, Azure Blob Storage). Az adatbázisban csak a kép elérési útját (URL-jét) tároljuk. Ez a megközelítés a legjobb teljesítmény, skálázhatóság és karbantarthatóság szempontjából.

---

### Szerveroldali Programozás: A Transact-SQL (T-SQL) Nyelv

**12-15. dia: Elsődleges kulcsok generálása és lekérdezése**

Az `IDENTITY` kulcsszóval automatikusan sorszámozott elsődleges kulcsot hozhatunk létre.

```sql
-- Létrehozunk egy táblát, ahol az ID oszlop automatikusan
-- növekedni fog 1-től, 1-esével.
CREATE TABLE Statusz (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Nev NVARCHAR(20)
);

-- Beszúrunk egy új sort, az ID értékét nem kell megadnunk,
-- a rendszer automatikusan generálja.
INSERT INTO Statusz (Nev) VALUES ('Kész');
```

A fő probléma, hogy a beszúrás után hogyan kapjuk vissza a frissen generált `ID`-t. Három függvény van erre, de **csak egy a biztonságos**:

1.  **`@@IDENTITY` (VESZÉLYES):** Visszaadja a *kapcsolaton (session)* belül legutoljára generált ID-t. Ha a `Statusz` táblába való beszúrás elindít egy triggert, ami beszúr egy másik táblába is, akkor `@@IDENTITY` a trigger által generált ID-t fogja visszaadni!
2.  **`SCOPE_IDENTITY()` (BIZTONSÁGOS):** Visszaadja az aktuális *hatókörön (scope)* belül legutoljára generált ID-t. A scope a mi kódunk (pl. a tárolt eljárás), a trigger egy másik scope-nak számít. **Ezért szinte mindig ezt kell használni!**
3.  **`IDENT_CURRENT('Táblanév')`:** Visszaadja a megadott táblába legutoljára generált ID-t, függetlenül attól, hogy melyik session vagy scope generálta.

**15. dia: `@@IDENTITY` vs. `SCOPE_IDENTITY` példa:**
Képzeld el, hogy a `T1` táblán van egy trigger, ami minden `INSERT` után automatikusan beszúr egy naplóbejegyzést a `T2` táblába.

```sql
-- Beszúrunk a T1 táblába. Tegyük fel, az új ID itt 100 lesz.
-- A trigger lefut, és beszúr a T2 táblába, ahol az új ID 5000 lesz.
INSERT INTO T1 (adat) VALUES ('valami');

SELECT @@IDENTITY;       -- Eredmény: 5000 (a trigger által generált ID a T2-ből) - HELYTELEN!
SELECT SCOPE_IDENTITY(); -- Eredmény: 100 (a mi INSERT-ünk által generált ID a T1-be) - HELYES!
```

**16-18. dia: Tranzakciók, Izolációs szintek, Hozzáférés**
Ezek a diák az SQL Server konkrét beállításait mutatják be a korábban tanult elméleti fogalmakhoz.
*   **Tranzakció indítása:** Alapból minden utasítás önálló tranzakció (`Auto commit`). Ha több utasítást akarunk egyben kezelni, használjuk az `EXPLICIT` módot: `BEGIN TRAN ... COMMIT/ROLLBACK TRAN`.
*   **Izolációs szintek:** Az MSSQL alapértelmezett szintje a `Read committed`.
*   **Hozzáférés szabályozás:** A `GRANT` (engedélyez), `DENY` (tilt) és `REVOKE` (visszavon) parancsokkal nagyon finomhangolt jogosultsági rendszert lehet kiépíteni.

**19-26. dia: A Szerveroldali Programozás Motivációja és Előnyei/Hátrányai**
A lényeg, hogy bizonyos logikákat (főleg amik versenyhelyzetet okozhatnak, mint a vizsgajelentkezés) biztonságosabb és hatékonyabb az adatbázis-szerveren, egyetlen atomi tranzakcióban futtatni.

Itt a vizsgajelentkezős példa egy **Tárolt Eljárásként (Stored Procedure)** megvalósítva:

```sql
-- Létrehozunk egy tárolt eljárást, ami két bemeneti paramétert vár.
CREATE PROCEDURE vizsgajelentkezes
    @vizsgaid INT,
    @hallgatoid INT
AS
BEGIN -- Az eljárás törzsének kezdete
    -- Deklarálunk (létrehozunk) egy lokális változót az aktuális jelentkezők számának.
    DECLARE @jelentezettek_db INT;
    -- Deklarálunk egy változót a vizsga maximális létszámának.
    DECLARE @vizsgakorlat INT;

    -- Megszámoljuk, hányan jelentkeztek már erre a vizsgára, és az eredményt beletesszük a változónkba.
    SELECT @jelentezettek_db = COUNT(*) FROM vizsgajelentkezes WHERE ID = @vizsgaid;

    -- Lekérdezzük a vizsga maximális létszámát a Vizsga táblából.
    SELECT @vizsgakorlat = maxletszam FROM vizsga WHERE ID = @vizsgaid;

    -- Elágazás: megnézzük, van-e még szabad hely.
    IF @jelentezettek_db < @vizsgakorlat
    BEGIN
        -- Ha van hely, beszúrjuk a hallgató jelentkezését.
        INSERT INTO vizsgajelentkezes (VizsgaID, HallgatoID) VALUES (@vizsgaid, @hallgatoid);
    END
    ELSE
    BEGIN
        -- Ha nincs hely, hibát dobunk egy egyedi hibakóddal és üzenettel.
        THROW 51005, 'A megengedett létszám betelt!', 1;
    END
END -- Az eljárás törzsének vége
```

**27-39. dia: A T-SQL Nyelv és a Kurzorok**
A T-SQL lehetővé teszi a procedurális programozást az adatbázisban.

A **kurzor** a halmazalapú SQL-szemlélet "antitézise", soronkénti feldolgozást tesz lehetővé. **Használatát kerülni kell, ha lehetséges, mert általában lassú.** Csak akkor indokolt, ha egy összetett, soronként eltérő logikát kell végrehajtani.

Egy tipikus kurzor használatának váza:
```sql
-- 1. Deklaráció: Definiáljuk a kurzort és a hozzá tartozó lekérdezést.
-- Végig akarunk menni azokon a termékeken, amiből kevesebb, mint 3 van raktáron.
DECLARE products_cur CURSOR FOR
    SELECT Id, Name FROM Product WHERE Stock < 3;

-- Deklaráljuk a változókat, amikbe a sorok értékeit fogjuk beolvasni.
DECLARE @ProductID INT, @ProductName NVARCHAR(100);

-- 2. Megnyitás: A lekérdezés lefut, a kurzor készen áll a bejárásra.
OPEN products_cur;

-- 3. Első sor beolvasása (Fetch): A ciklus előtt be kell olvasni az első elemet.
FETCH NEXT FROM products_cur INTO @ProductID, @ProductName;

-- 4. Ciklus: Addig megyünk, amíg a FETCH sikeresen tud sort beolvasni.
-- A @@FETCH_STATUS globális változó értéke 0, ha a FETCH sikeres volt.
WHILE @@FETCH_STATUS = 0
BEGIN
    -- Itt történik a soronkénti feldolgozás.
    -- Pl. kiírjuk a termék nevét.
    PRINT 'Feldolgozás alatt: ' + @ProductName;

    -- A ciklus végén beolvassuk a KÖVETKEZŐ sort. Ez a lépés kritikus!
    FETCH NEXT FROM products_cur INTO @ProductID, @ProductName;
END

-- 5. Bezárás: Felszabadítja az eredményhalmazt.
CLOSE products_cur;

-- 6. Deallokálás: Felszabadítja a kurzorhoz lefoglalt memóriát.
DEALLOCATE products_cur;
```

**40-46. dia: Tárolt eljárások és függvények**
*   **Tárolt eljárás (`PROCEDURE`):** Módosíthat adatot, lehetnek be- és kimeneti paraméterei, de nincs közvetlen visszatérési értéke. Meghívása: `EXEC eljárás_neve paraméter1, ...`.
*   **Függvény (`FUNCTION`):** **Csak olvashat** adatot, és **kötelezően van visszatérési értéke**. Meghívása `SELECT dbo.függvény_neve(paraméter)`.
    *   **Skalár függvény:** Egyetlen értéket ad vissza.

        ```sql
        -- Létrehoz egy függvényt, ami visszaadja a legnagyobb ÁFA-kulcsot.
        CREATE FUNCTION dbo.LargestVATPercentage()
        RETURNS INT -- Megadjuk a visszatérési érték típusát.
        AS
        BEGIN
            RETURN (SELECT MAX(Percentage) FROM VAT); -- A RETURN utasítással adjuk vissza az értéket.
        END
        ```
    *   **Tábla-visszatérésű függvény:** Egy teljes táblát ad vissza, amit utána egy `FROM` klózban fel lehet használni.

**47-52. dia: Hibakezelés**
A modern, javasolt módszer a `TRY...CATCH` blokk.

```sql
BEGIN TRY
    -- Ide jön a kód, amiben hiba történhet.
    -- Pl. megpróbálunk nullával osztani, ami hibát fog generálni.
    SELECT 1 / 0;
END TRY
BEGIN CATCH
    -- Ha a TRY blokkban hiba történik, a futás itt folytatódik.
    -- A CATCH blokkban le tudjuk kérdezni a hiba részleteit.
    SELECT
        ERROR_NUMBER() AS Hibakód,
        ERROR_MESSAGE() AS Hibaüzenet,
        ERROR_LINE() AS HibásSorSzáma;

    -- Opcionálisan tovább is dobhatjuk a hibát, ha a hívónak is tudnia kell róla.
    -- THROW;
END CATCH
```

A `THROW` paranccsal mi magunk is generálhatunk hibát.

```sql
-- A vizsgajelentkezős eljárásunkban, ha betelt a létszám:
THROW 51000, 'A vizsgára a létszám betelt!', 1;
-- 51000: egyedi hibakód (50000 felett kell lennie)
-- '...': a hibaüzenet
-- 1: a hiba állapota (state)
```

**53-60. dia: Triggerek**
A triggerek olyan eljárások, amik adatbázis-eseményekre (pl. `INSERT`) automatikusan aktiválódnak.

A legfontosabb a `deleted` és `inserted` speciális, memóriabeli táblák ismerete. Ezek tartalmazzák a módosítás előtti és utáni sorokat.

Példa egy audit naplózó triggerre, ami minden terméktörlésről bejegyzést tesz egy `AuditLog` táblába:

```sql
-- Először létrehozzuk a naplótáblát.
CREATE TABLE AuditLog (
    Description NVARCHAR(MAX) NULL
);
GO -- Új batch indítása

-- Létrehozzuk a triggert a Product táblára, ami a DELETE eseményre fog lefutni.
CREATE TRIGGER ProductDeleteLog
ON Product       -- Ezen a táblán fog működni.
FOR DELETE      -- Erre az eseményre.
AS
BEGIN
    -- Ne feledjük: egy DELETE utasítás több sort is törölhet egyszerre!
    -- A triggernek erre fel kell készülnie.
    -- A 'deleted' tábla tartalmazza az ÖSSZES törölt sort.

    -- Beszúrjuk az AuditLog táblába az összes törölt termék nevét.
    -- A SELECT...FROM deleted lekérdezés végigmegy a törölt sorokon.
    INSERT INTO AuditLog (Description)
    SELECT 'Termék törölve: ' + d.Name
    FROM deleted d; -- A 'deleted' táblát 'd' aliasnéven használjuk.
END
```
Ha most kiadunk egy `DELETE FROM Product WHERE ID IN (1, 5, 10);` parancsot, a trigger egyszer fog lefutni, és a `deleted` táblában 3 sor lesz, így az `AuditLog` táblába 3 új bejegyzés kerül.

**61-62. dia: Érdekesség: batch**
A `GO` parancs nem része a T-SQL nyelvnek. Ez egy kliensoldali parancs, ami az SQL Server Management Studionak (SSMS) vagy más eszközöknek szól. Azt jelenti: "Az eddig beírt parancsokat küldd el egy csomagban (batch) a szervernek, és utána kezdj egy újat." Emiatt egy `GO` előtti változó nem látható egy `GO` utáni kódrészletben, mert azok két külön végrehajtási körben futnak le.