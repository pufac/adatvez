Természetesen, itt van a harmadik diasor teljes körű, minden részletre kiterjedő és kódkommentárokkal ellátott magyarázata. Ez a prezentáció az adatelérési réteg (Data Access Layer - DAL) magasabb szintű architekturális kérdéseivel foglalkozik.

---

### Bevezetés és az Adatelérési Réteg Feladata (1-3. dia)

**1. dia: Címlap**
Az előadás címe: **"Adatvezérelt rendszerek: Objektum-relációs leképzés, Konkurencia kezelés, Adatelérési rétegben használt minták"**. Ez a három fő téma adja a prezentáció gerincét, amelyek mind az alkalmazásunk (üzleti logikai réteg) és az adatbázis közötti kommunikációt és annak problémáit járják körül.

**2. dia: Adatelérési réteg feladata**
Ez a dia összefoglalja az adatelérési réteg (DAL) három alapvető felelősségét a szoftverarchitektúrában:
1.  **Elemi adatelérés biztosítása:** Ez a réteg felelős az alapvető adatbázis-műveletek (CRUD: Create, Read, Update, Delete) végrehajtásáért. Az üzleti logikának nem kell tudnia, hogyan kell SQL parancsokat írni, csak annyit kell mondania a DAL-nak, hogy "mentsd el ezt az ügyfelet".
2.  **Konkurenciakezelés:** Kezelni a helyzetet, amikor több felhasználó egyszerre próbálja ugyanazt az adatot módosítani.
3.  **Adatelérési absztrakció:** Elrejteni az adatbázis-kezelés konkrét technikai részleteit az alkalmazás többi része elől. Az üzleti logikát nem szabad, hogy érdekelje, hogy az adatokat egy MS SQL, egy Oracle vagy egy fájlrendszer tárolja-e.

**3. dia:** Ez egy tartalomjegyzék-szerű áttekintő dia, ami vizuálisan csoportosítja a prezentáció témaköreit.

---

### I. Rész: Objektum-Relációs Leképzés (ORM) (4-43. dia)

Ez a fejezet az "objektum-relációs impedancia-különbség" problémájával foglalkozik: az objektumorientált programozás (OOP) világa és a relációs adatbázisok világa alapvetően másképp modellezi a valóságot. Az ORM eszközök (pl. Entity Framework, Hibernate) ezt a szakadékot próbálják áthidalni.

**4-7. dia: Probléma felvetés és a két világ modellezése**
*   **Üzleti logika (OOP világ):** Osztályokban, objektumokban, öröklődésben, viselkedésben (metódusokban) és komplex kapcsolatokban gondolkodik. A modellezés eszköze pl. az UML.
*   **Adatréteg (Relációs világ):** Táblákban, sorokban, oszlopokban és külső kulcsos kapcsolatokban gondolkodik. A modellezés eszköze az E/R (Entity-Relationship) diagram. A szemlélete statikus, csak az adatstruktúrát írja le.

Az ORM feladata lefordítani az üzleti objektumokat (pl. egy `Customer` osztályt) a relációs adatbázis tábláira és vissza.

**10-12. dia: Alap koncepció és azonnali problémák**
Az alapötlet egyszerűnek tűnik:
*   Osztály → Tábla
*   Adattag (Property) → Oszlop
*   Objektumok közötti kapcsolat → Külső kulcs (Foreign Key)

De a problémák azonnal megjelennek:
*   **Összetett mezők:** Az OOP-ban egy `Customer` objektumnak lehet egy `Address` típusú tulajdonsága, ami maga is egy objektum (irányítószámmal, várossal). Ezt a relációs világban nem lehet egyetlen oszlopba tenni. A megoldás általában az, hogy az `Address`-nek külön táblát hozunk létre, és külső kulccsal hivatkozunk rá.
*   **Eltérő adattípusok:** Az alkalmazás `string` típusa az adatbázisban lehet `nvarchar(255)`, `varchar(max)` vagy `text`. Ezt a konverziót kezelni kell.
*   **Shadow információk:** Az adatbázisnak szüksége van olyan "technikai" adatokra, amik az üzleti modellben feleslegesek. A legfontosabb ilyen az **elsődleges kulcs (`ID`)**. De ide tartozhatnak a verziószámok vagy időbélyegek is, amik a konkurenciakezeléshez kellenek. Az ORM-nek ezeket is kezelnie kell anélkül, hogy "beszennyezné" az üzleti objektumokat.

**13-33. dia: Az öröklődés leképzése – A legnehezebb probléma**
Az öröklődés az OOP egyik alappillére, de a relációs adatbázisoknak nincs rá beépített megfelelője. Az előadás 4 fő stratégiát mutat be ennek a leképzésére.

**1. Stratégia: Table Per Hierarchy (TPH) - Egy táblába történő leképezés** (16-18. dia)
*   **Ötlet:** A teljes osztályhierarchia (ősosztály és összes leszármazottja) egyetlen nagy táblába kerül. Minden lehetséges attribútum külön oszlopot kap.
*   **Megvalósítás:** Kell egy plusz oszlop, az ún. **diszkriminátor (discriminator)**, ami megmondja, hogy az adott sor melyik konkrét osztálynak felel meg (pl. 'Customer' vagy 'Employee').
*   **Előnyök:** Egyszerű, gyors lekérdezés (nincs szükség `JOIN`-okra), könnyű új osztályt hozzáadni (csak új oszlopok kellenek).
*   **Hátrányok:** Helypazarlás a sok `NULL` érték miatt (egy `Customer` sornak a `Salary` oszlopa `NULL` lesz). A kötelezőséget (`NOT NULL`) nehéz a leszármazottakra kikényszeríteni.
*   **Mikor jó?** Egyszerű, nem túl mély hierarchiáknál.

**2. Stratégia: Table Per Concrete Class (TPC) - Minden valós osztály leképzése saját táblába** (19-21. dia)
*   **Ötlet:** Csak a konkrét, példányosítható osztályok kapnak táblát. Az absztrakt ősosztályok nem.
*   **Megvalósítás:** Minden tábla tartalmazza a saját és az összes őstől örökölt attribútumot is. Az adatok duplikálódnak.
*   **Előnyök:** Átláthatóbb, gyors adatelérés egy konkrét típusra (nincs `JOIN`).
*   **Hátrányok:** Ha az ősosztály megváltozik (pl. kap egy új mezőt), az összes leszármazott tábláját módosítani kell. Nehéz lekérdezni az összes `Person`-t, mert `UNION`-nal kell összekapcsolni a `Customer` és `Employee` táblákat.
*   **Mikor jó?** Ha ritkán változik a struktúra, és a lekérdezések általában csak egy konkrét típusra irányulnak.

**3. Stratégia: Table Per Type (TPT) - Minden osztály leképzése saját táblába** (22-24. dia)
*   **Ötlet:** A hierarchia minden osztálya (az absztrakt is) kap egy saját táblát.
*   **Megvalósítás:** A `Person` tábla tartalmazza a közös adatokat (`ID`, `Name`). A `Customer` és `Employee` táblák csak a saját attribútumaikat tartalmazzák, és egy külső kulccsal (`PID (FK)`) hivatkoznak a `Person` tábla megfelelő sorára. Az ID-k megegyeznek.
*   **Előnyök:** Nincs helypazarlás (`NULL` értékek), normalizált, tiszta adatmodell, ami a legjobban tükrözi az OO hierarchiát.
*   **Hátrányok:** Egy konkrét objektum (pl. egy `Employee`) adatainak lekérdezéséhez `JOIN`-olni kell a táblákat (`Employee` JOIN `Person`), ami lassabb lehet.
*   **Mikor jó?** Komplex, sokat változó hierarchiák esetén.

**4. Stratégia: Leképezés általános struktúrába (Metadata Driven)** (25-28. dia)
*   **Ötlet:** Ahelyett, hogy az osztályainkat képeznénk le, létrehozunk egy meta-modellt, ami képes leírni bármilyen osztálystruktúrát.
*   **Megvalósítás:** Ez egy Entity-Attribute-Value (EAV) modell. Vannak tábláink az osztályok (`Class`), az attribútumok (`Attribute`), és az értékek (`Value`) tárolására. Az `Inheritance` tábla pedig az öröklődési kapcsolatokat írja le.
*   **Előnyök:** Rendkívül flexibilis, futásidőben is lehet új osztályokat, attribútumokat hozzáadni az adatbázis-séma módosítása nélkül.
*   **Hátrányok:** Nagyon bonyolult, nehéz "összeszedni" egy objektum adatait, és nagy adatmennyiségnél lassú.
*   **Mikor jó?** Olyan rendszereknél, ahol a felhasználók maguk definiálhatnak saját adattípusokat (pl. egy CMS rendszer).

**34-42. dia: Kapcsolatok és Rekurzió leképzése**
*   **Kapcsolatok típusai:** 1-1, 1-N (egy-több), N-N (több-több).
*   **Leképezés:**
    *   **1-1 és 1-N:** Külső kulccsal az "N" oldali táblában.
    *   **N-N:** Közvetlenül nem képezhető le. Szükség van egy harmadik, ún. **kapcsoló táblára (junction table)**, ami mindkét eredeti tábla elsődleges kulcsát tartalmazza külső kulcsként.
*   **Rekurzió (reflexív kapcsolat):** Amikor egy osztály saját magával van kapcsolatban (pl. egy `Employee`-nek van egy `Manager`-e, aki szintén egy `Employee`). Ezt is egy külső kulccsal képezzük le, ami ugyanannak a táblának az elsődleges kulcsára hivatkozik (`ManagerEmployeeID` FK → `EmployeeID` PK).

---

### II. Rész: Konkurenciakezelés (50-64. dia)

Ez a rész a "lost update" (elveszett módosítás) problémáját vizsgálja az alkalmazás réteg szemszögéből, különösen állapotmentes (stateless) környezetben, mint a web.

**51-54. dia: A probléma bemutatása**
1.  Felhasználó A lekérdezi a terméket. Az ár a képernyőjén: 9875. Átírja 9999-re.
2.  Eközben Felhasználó B is lekérdezi ugyanazt a terméket. Az ő képernyőjén is 9875 az ár. Átírja 8888-ra.
3.  Felhasználó A elmenti a módosítását. Az adatbázisban az ár 9999 lesz.
4.  Felhasználó B elmenti a módosítását. Az adatbázisban az ár 8888 lesz. **Felhasználó A módosítása elveszett!**

**55-64. dia: Megoldási stratégiák**

**1. Pesszimista konkurenciakezelés:**
*   **Filozófia:** "Biztosan lesz ütközés, ezért előre zárolom az adatot."
*   **Működése:** Amikor egy felhasználó elkezdi szerkeszteni a rekordot, a rendszer zárolja azt az adatbázisban, így más nem férhet hozzá.
*   **Probléma:** Webes alkalmazásokban nem működik jól, mert a felhasználó órákig is nyitva tarthat egy szerkesztő ablakot, addig pedig senki más nem férne hozzá az adathoz.

**2. Optimista konkurenciakezelés:**
*   **Filozófia:** "Valószínűleg nem lesz ütközés. Engedek mindenkit szerkeszteni, de mentés előtt ellenőrzöm, hogy történt-e közben változás."
*   **Működése (Rekord verzióval):**
    1.  Minden táblához adunk egy `Version` (vagy `timestamp`/`rowversion`) oszlopot.
    2.  Amikor a felhasználó betölti az adatot (pl. a 123-as terméket, `Version=3`), az alkalmazás eltárolja a verziószámot is.
    3.  Mentéskor az alkalmazás egy feltételes `UPDATE` parancsot küld:

        ```sql
        -- Próbáljuk meg frissíteni az árat 9999-re, ÉS növelni a verziót 4-re,
        -- DE CSAK AKKOR, ha a termék ID-ja 123 ÉS a verziója MÉG MINDIG 3.
        UPDATE Products
        SET Price = 9999, Version = 4
        WHERE ID = 123 AND Version = 3;
        ```
    4.  Az alkalmazás ellenőrzi, hány sor módosult.
        *   Ha **1 sor módosult**, a mentés sikeres volt.
        *   Ha **0 sor módosult**, az azt jelenti, hogy valaki más már módosította a rekordot (a `Version` már nem 3), tehát **ütközés történt!**
*   **Ütközés feloldása:** Ha ütközés van, az alkalmazásnak döntenie kell:
    *   **Az utolsó író nyer (Last one wins):** Ez a legrosszabb, mert felülírja a másik munkáját.
    *   **Az első író nyer (First one wins):** Az optimista zárolás ezt valósítja meg. A második író hibaüzenetet kap.
    *   **A felhasználóra bízzuk:** A program egy ablakban megmutatja a két verziót ("Az adatokat közben más módosította. Mit szeretnél tenni? Felülírod / Visszavonod a saját módosításaidat?").

---

### III. Rész: Adatelérési Rétegben Használt Minták (65-95. dia)

Ez a rész bemutatja a **Repository** tervezési mintát, ami egy kulcsfontosságú absztrakciós eszköz.

**65-74. dia: A Repository Minta bemutatása**
*   **Probléma:** Mi van, ha az alkalmazásunknak több különböző adattárolót is kell használnia (MS SQL, Oracle, NoSQL)? Hogyan tudjuk az üzleti logikát függetleníteni ezektől?
*   **Megoldás:** A **Repository Pattern**.
    *   **Célja:** Egy "memóriában lévő gyűjtemény" illúzióját kelteni az üzleti logika számára. A BLL úgy kérhet el vagy adhat hozzá objektumokat, mintha csak egy listával dolgozna, és nem kell tudnia a mögöttes adatbázis-technológiáról.
    *   **Megvalósítás:** Létrehozunk egy interfészt (pl. `IProductRepository`), ami definiálja a műveleteket (`GetById`, `GetAll`, `Add`, `Delete`). Az üzleti logika csak ezt az interfészt ismeri. Ezután létrehozunk egy konkrét osztályt (pl. `MsSqlProductRepository`), ami megvalósítja ezt az interfészt, és tartalmazza a konkrét, MS SQL-specifikus adatelérési kódot. Ha később Oracle-re kell váltanunk, csak egy új `OracleProductRepository` osztályt kell írnunk, az üzleti logika nem változik.

    ```csharp
    // A Repository interfész, amit az Üzleti Logika (BLL) ismer.
    public interface IProductRepository
    {
        Product FindById(int id);
        List<Product> List();
        void Add(Product entity);
        void Update(Product entity);
        void Delete(Product entity);
    }

    // A konkrét implementáció, ami elrejti az Entity Framework (EF) vagy más DB technológia részleteit.
    public class ProductRepository : IProductRepository
    {
        private readonly MyDbContext _context; // Pl. egy EF DbContext

        public ProductRepository(MyDbContext context)
        {
            _context = context;
        }

        public Product FindById(int id)
        {
            // Konkrét DB hívás...
            return _context.Products.Find(id);
        }
        // ... a többi metódus implementációja ...
    }
    ```

**75-95. dia: Az ORM és a Repository viszonya, kritikája**
Ez egy híres vita a szoftverfejlesztésben: **Szükség van-e külön Repository rétegre, ha már használunk egy ORM-et (pl. Entity Framework), ami maga is egy absztrakció?**

*   **Az EF már egy Repository?** Az EF `DbContext`-je és `DbSet`-jei nagyon hasonlítanak egy Repository-ra. Lehet rajtuk keresztül lekérdezni, hozzáadni, törölni.
*   **A "szivárgó absztrakció" (Leaky Abstraction) problémája:**
    *   Ha a Repository egy `IQueryable<T>` típust ad vissza, az lehetővé teszi, hogy az üzleti logika tovább finomítsa a lekérdezést (pl. `.Where(...)`, `.OrderBy(...)`).
    *   Ez kényelmes, de **megsérti a rétegek szétválasztását**, mert a lekérdezési logika (ami adatbázis-specifikus lehet) "átszivárog" az üzleti rétegbe.
*   **Mikor érdemes Repository-t használni EF fölött? (Előnyök)**
    1.  **Cserélhetőség:** Ha reális esély van rá, hogy adatbázis-technológiát kell váltani (pl. SQL-ről NoSQL-re).
    2.  **Unit tesztelés:** A Repository interfészt könnyű "kigúnyolni" (mockolni), így az üzleti logika tesztelhető anélkül, hogy valódi adatbázisra lenne szükség.
    3.  **Domain-specifikus API:** A Repository-ban létrehozhatunk beszédes nevű metódusokat (`GetActivePremiumCustomers()`), ami tisztábbá teszi az üzleti logikát, mint a hosszú LINQ láncok.
*   **Mikor lehet felesleges? (Kritika)**
    *   **YAGNI (You Aren't Gonna Need It):** Ha sosem fogod lecserélni az adatbázist, akkor a plusz réteg csak felesleges bonyolítás.
    *   **Bonyolítja a kódot:** Minden egyes lekérdezéshez külön metódust kell írni a Repository-ba.
    *   **Elveszti az EF erejét:** A `IQueryable` nyújtotta rugalmasság és a LINQ teljes ereje elvész.

**Irányelv (94. dia):** Ne ragaszkodj a nevekhez (`DAL`, `BLL`, `Repo`). A cél a **felelősségek szétválasztása** és a **tesztelhetőség**. A bonyolult lekérdezési logika tartozzon az adatréteghez (pl. egy Repository-ba). A memóriába betöltött adatokon végzett üzleti logika pedig tartozzon az üzleti rétegbe (pl. egy külön "Service" vagy "Manager" osztályba).