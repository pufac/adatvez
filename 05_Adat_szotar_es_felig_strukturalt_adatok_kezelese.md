Rendben, következzen az ötödik diasor teljes, részletes és kommentált feldolgozása, ami az adatszótárakra és a félig strukturált adatok (XML, JSON) kezelésére fókuszál.

---

### Bevezetés és Tartalom (1-2. dia)

**1. dia: Címlap**
Az előadás címe: **"Adatvezérelt rendszerek: Adatszótárak, Félig strukturált adatok (XML, JSON)"**. Ez a rész két fontos, de eltérő témát jár körül:
1.  **Adatszótárak:** Hogyan tárolja maga az adatbázis a saját szerkezetére vonatkozó információkat (metaadatokat).
2.  **Félig strukturált adatok:** Hogyan kezeljük azokat az adatokat, amelyek nem illeszkednek a relációs táblák merev sor-oszlop struktúrájába.

**2. dia: Tartalom**
Ez a dia vizuálisan vázolja fel a prezentáció öt fő blokkját:
1.  Relációs adatbázisok adatszótára.
2.  Félig strukturált adatok kezelése: XML alapok.
3.  Félig strukturált adatok kezelése: JSON alapok.
4.  XML kezelése relációs adatbázisokban.
5.  JSON kezelése relációs adatbázisokban.

---

### I. Rész: Relációs Adatbázisok Adatszótára (3-10. dia)

**3. dia:** Elválasztó dia.

**4-5. dia: Mire való az adatszótár?**
A dia egy SQL kódrészlettel indít, ami megmutatja a gyakorlati hasznát:

```sql
-- Ellenőrizzük, hogy létezik-e már az 'Invoice' nevű tábla.
IF EXISTS (
    SELECT *
    FROM sys.objects -- A 'sys.objects' egy adatszótár nézet, ami minden adatbázis-objektumot listáz.
    WHERE object_id = OBJECT_ID(N'[dbo].[Invoice]') -- Lekérjük az 'Invoice' tábla azonosítóját.
      AND type IN (N'U') -- 'U' = User Table (Felhasználói tábla).
)
-- Ha a tábla létezik, akkor töröljük.
DROP TABLE Invoice;

-- Ezután (újra) létrehozzuk a táblát.
CREATE TABLE [Invoice](...);
```

*   **Miért van erre szükség?** Azért, hogy **idempotens** szkripteket tudjunk írni.
*   **Idempotens:** Olyan művelet vagy szkript, amit akárhányszor lefuttatunk, az eredmény mindig ugyanaz lesz. Ez a szkript létrehozza az `Invoice` táblát, függetlenül attól, hogy az korábban létezett-e vagy sem. Ez elengedhetetlen az automatizált telepítési és frissítési folyamatoknál.
*   **Az adatszótár szerepe:** Lehetővé teszi, hogy programozottan lekérdezzük az adatbázis saját struktúráját (metaadatait), és ez alapján hozzunk döntéseket a szkripten belül.

**6. dia: Adatszótár (Data Dictionary)**
*   **Definíció:** Az adatbázisban tárolt "adat az adatról", azaz **metaadat**. Ez egy központi tárháza az adatbázis struktúrájára, tartalmára és szabályaira vonatkozó információknak.
*   **Integrált rész:** Nem egy különálló dokumentum, hanem az adatbázis-kezelő szerves része.
*   **Megvalósítás:** Általában speciális, csak olvasható **rendszernézetek (system views)** formájában érhető el, amiket ugyanúgy le lehet kérdezni SQL-lel, mint a normál táblákat.

**7. dia: Adatszótár tartalma**
Mit tárol az adatszótár? Lényegében mindent, ami az adatbázist leírja:
*   **Séma objektumok:** Táblák, oszlopok (nevük, típusuk), nézetek, indexek, tárolt eljárások stb. leírása.
*   **Integritási kritériumok:** Elsődleges és külső kulcsok, `CHECK` kényszerek.
*   **Biztonság:** Felhasználók, szerepkörök és azok jogosultságai.
*   **Monitoring:** Futásidejű információk, pl. aktív kapcsolatok, zárolások (lock-ok).
*   **Auditing:** Információk arról, hogy ki és mikor módosította az adatbázis struktúráját.

**8-9. dia: MS SQL Adatszótár**
Az MS SQL Serverben három fő csoportja van a rendszernézeteknek:
1.  **Information Schema Views:** (pl. `INFORMATION_SCHEMA.TABLES`, `INFORMATION_SCHEMA.COLUMNS`)
    *   Ezek az **ISO szabványnak** megfelelő nézetek.
    *   Előnyük, hogy (elvileg) adatbázis-függetlenek, tehát egy PostgreSQL-ben is hasonló nézeteket találunk.
    *   Kevésbé részletesek, mint a Catalog View-k.
2.  **Catalog Views:** (pl. `sys.tables`, `sys.objects`, `sys.columns`)
    *   Ezek a Microsoft **SQL Server-specifikus** nézetek.
    *   Sokkal részletesebb, teljes körű információt nyújtanak a szerverről és az adatbázisról. Általában ezeket használjuk, ha MSSQL-ben dolgozunk.
3.  **Dynamic Management Views (DMVs):** (pl. `sys.dm_tran_locks`)
    *   Ezek nem a statikus sémát, hanem a szerver **aktuális, futásidejű állapotát** írják le.
    *   Diagnosztikára, teljesítményelemzésre használatosak (pl. melyik lekérdezés fut éppen, milyen zárolások aktívak).

**10. dia: Példák az adatszótár használatára**
Ez a dia két tipikus "karbantartó" szkriptet mutat be:
*   **Tábla törlése, ha létezik:** Ugyanaz a példa, mint az elején, csak most a `sys.objects`-ot használva.
*   **Oszlop hozzáadása, ha még nem létezik:**

    ```sql
    -- Ellenőrizzük, hogy a 'Product' táblában létezik-e már a 'Description' nevű oszlop.
    IF NOT EXISTS (
        SELECT *
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'Product' AND COLUMN_NAME = 'Description'
    )
    -- Ha nem létezik, adjuk hozzá a táblához egy 'xml' típusú oszlopként.
    ALTER TABLE Product ADD Description xml;
    ```
    Ez is egy idempotens szkript, ami biztosítja, hogy a `Description` oszlop létezzen, de nem okoz hibát, ha már korábban hozzáadtuk.

---

### II. Rész: Félig Strukturált Adatok - XML (11-31. dia)

**11-14. dia: XML Alapok**
*   **XML (Extensible Markup Language):** Szöveges, platformfüggetlen adatábrázolási formátum.
*   **Önleíró:** A `<tagek>` elnevezése hordozza az adat jelentését (szemantikáját). Ember és gép által is olvasható.
*   **Struktúra:**
    *   `<?xml ... ?>`: XML deklaráció (opcionális, de javasolt).
    *   `<elem attributum="érték"> ... </elem>`: Elemek, attribútumok.
    *   `<!-- ... -->`: Komment.
    *   `<![CDATA[ ... ]]>`: Olyan szakasz, amiben bármilyen szöveg szerepelhet anélkül, hogy a parser értelmezni próbálná (pl. HTML kódrészletet tehetünk bele).

**15-17. dia: Encoding és Névterek**
*   **Encoding:** Meghatározza, hogyan lesznek a karakterek bájtokká alakítva. A leggyakoribb a `UTF-8`.
*   **Névterek (Namespaces):** Céljuk az elemnevek közötti ütközések elkerülése. Ha két különböző XML formátumot keverünk (pl. HTML és XSLT), egy névtér (`xmlns:xsl="..."`) segít megkülönböztetni, hogy egy `<template>` tag az XSLT-hez tartozik-e.

**18. dia: XML Előnyök / Hátrányok**
*   **Előnyök:** Szabványos, platformfüggetlen, séma-leírható (XSD), típusos lehet.
*   **Hátrányok:** "Bőbeszédű" (nagyobb a mérete, mint a JSON-nak), nem mindig egyértelmű, hogy egy adatot attribútumként vagy gyerek elemként reprezentáljunk-e.

**19-22. dia: XML .NET-ben és a DOM**
*   **.NET-ben:** Az `XmlSerializer` osztály segítségével könnyen lehet C# objektumokat XML-lé (szerializáció) és XML-ből vissza (deszerializáció) alakítani. Attribútumokkal (`[XmlElement]`, `[XmlAttribute]`) testreszabható a folyamat.
*   **DOM (Document Object Model):** Az XML feldolgozásának egyik módja. A parser beolvassa a teljes XML dokumentumot a memóriába, és egy fa-szerkezetű objektummodellt épít fel belőle, amin utána programozottan navigálhatunk.

**22-31. dia: XPath - XML Lekérdező Nyelv**
Az XPath egy nyelv kifejezetten az XML dokumentum (DOM fa) részeinek a megcímzésére és lekérdezésére.
*   **Szintaxis:** Útvonal-kifejezésekkel navigál a fa-struktúrában.
    *   `/`: Gyökértől indul.
    *   `//`: Bárhol a dokumentumban keres.
    *   `.`: Aktuális csomópont.
    *   `..`: Szülő csomópont.
    *   `@`: Attribútumot jelöl.
    *   `[...]`: Szűrési feltétel.

Példák a dián lévő XML-re:
*   `/konyvtar/konyv`: Kiválasztja az összes `konyv` elemet, ami a `konyvtar` gyökér-elem közvetlen gyereke.
*   `//cim`: Kiválasztja az összes `cim` elemet a dokumentumban, bárhol is legyenek.
*   `//cim[@nyelv='en']`: Kiválasztja azt a `cim` elemet, aminek van `nyelv` attribútuma, és annak értéke `'en'`.
*   `/konyvtar/konyv[1]`: Kiválasztja az első `konyv` elemet.
*   `/konyvtar/konyv[ar > 5000]`: Kiválasztja azokat a `konyv` elemeket, amelyeknek a gyerek `ar` elemének értéke nagyobb, mint 5000.

---

### III. Rész: Félig Strukturált Adatok - JSON (32-39. dia)

**32-34. dia: JSON Alapok**
*   **JSON (JavaScript Object Notation):** Könnyűsúlyú, szöveges, ember által olvasható adatcsere-formátum. Ma a webes API-k de facto szabványa.
*   **Alapelemei:**
    *   **Objektum (`{}`):** Kulcs-érték párok rendezetlen halmaza. A kulcsok stringek, idézőjelben.
    *   **Tömb (`[]`):** Értékek rendezett listája.
    *   **Érték:** Lehet string, szám, `true`, `false`, `null`, objektum vagy tömb.
*   **Problémák:** Nincs komment, a dátumoknak nincs szabványos formátuma (általában ISO 8601 stringként tárolják), és az `eval()` használata biztonsági kockázatot jelenthet.

**35-37. dia: JSON Schema és felhasználás**
*   **JSON Schema:** Lehetővé teszi a JSON adatok struktúrájának és típusainak leírását és validálását, hasonlóan az XML XSD-hez.
*   **Felhasználás:** Leggyakrabban backend-kliens (szerver-böngésző/mobilalkalmazás) kommunikációra használják, a REST API-k kedvelt formátuma. Egyre több adatbázis is támogatja natívan (pl. MongoDB, de a relációsak is).

**38-39. dia: JSON .NET-ben és XML vs. JSON**
*   **.NET-ben:** A modern, beépített könyvtár a `System.Text.Json`, ami a `JsonSerializer` osztállyal hatékony szerializációt és deszerializációt tesz lehetővé.
*   **Összehasonlítás:** A JSON általában kompaktabb (kisebb méretű), egyszerűbb a parsere, és jobban illeszkedik a modern webes technológiákhoz. Az XML erősebb a sémaleírásban, névterek kezelésében, és jobban elterjedt a dokumentum-orientált vállalati rendszerekben (pl. SOAP).

---

### IV-V. Rész: XML és JSON Kezelése Relációs Adatbázisokban (40-57. dia)

**40-47. dia: XML tárolása relációs adatbázisban**
*   **Motiváció:** Néha egy relációs tábla egy oszlopában szeretnénk rugalmas, változó struktúrájú adatokat tárolni (pl. egy termék változó paraméterei).
*   **MS SQL megoldás:** Két fő adattípus van erre:
    1.  **`nvarchar(max)`:** Egyszerű szövegként tárolja az XML-t. Az adatbázis nem tudja, hogy ez XML, nem validálja, és nem lehet hatékonyan keresni benne.
    2.  **`xml` (Javasolt):** Speciális adattípus. Az adatbázis:
        *   Ellenőrzi, hogy a beillesztett adat **jól formázott** XML-e.
        *   Lehetővé teszi séma (XSD) hozzárendelését, ami alapján **validálja** is az adatot.
        *   Speciális metódusokat (`.query()`, `.value()`, `.exist()`, `.modify()`) biztosít az XML-en belüli adatok **lekérdezésére és manipulálására** XPath/XQuery segítségével.
        *   Lehetővé teszi speciális **XML indexek** létrehozását a hatékony kereséshez.

**50-51. dia: XML lekérdezése és manipulálása**
Az MS SQL T-SQL nyelvét kiegészítették XML-kezelő metódusokkal:
*   `.query('/xpath/kifejezes')`: Egy XML részletet ad vissza.
*   `.value('(/xpath/kifejezes)[1]', 'SQL_tipus')`: Egyetlen skalár értéket von ki az XML-ből, és átalakítja a megadott SQL típussá.
*   `.exist('/xpath/kifejezes')`: 1-et ad vissza, ha a kifejezésnek van találata, egyébként 0-t.
*   `.modify('utasitas')`: Lehetővé teszi az XML tartalmának módosítását (`insert`, `delete`, `replace value of`).

**52. dia: `FOR XML`**
Ez a `SELECT` parancs egy speciális klóza, amivel egy lekérdezés eredményhalmazát lehet XML formátumba alakítani.

**53-57. dia: JSON kezelése MS SQL-ben**
Az MS SQL 2016-tól kezdve natív támogatást nyújt a JSON-hoz is, de másképp, mint az XML-hez.
*   **Nincs speciális `json` adattípus:** A JSON adatokat sima `NVARCHAR(MAX)` oszlopban kell tárolni.
*   **Beépített függvények:** A feldolgozáshoz egy sor beépített függvény áll rendelkezésre:
    *   `ISJSON(sztring)`: Ellenőrzi, hogy a szöveg érvényes JSON-e.
    *   `JSON_VALUE(sztring, '$.json.path')`: Egyetlen skalár értéket (szöveg, szám) von ki.
    *   `JSON_QUERY(sztring, '$.json.path')`: Egy beágyazott objektumot vagy tömböt von ki JSON szövegként.
    *   `OPENJSON(sztring)`: A legütősebb függvény, ami egy JSON objektumot vagy tömböt képes **relációs sorhalmazzá (táblává) alakítani**, amit utána `JOIN`-olni lehet más táblákkal.
    *   `FOR JSON`: Az XML-es megfelelője, egy `SELECT` lekérdezés eredményét alakítja JSON formátumúvá.

Az 56. és 57. dia bemutatja, hogyan lehet a `OPENJSON` és `FOR JSON` segítségével JSON adatokat lekérdezni és generálni.