Természetesen, következzen az utolsó előtti, lekérdezés-optimalizálásról szóló diasor teljes körű, minden részletre és kódrészletre kiterjedő feldolgozása. Ez egy nagyon fontos téma, ami a relációs és NoSQL adatbázisok "lelkébe" enged betekintést.

---

### Bevezetés és a Cél (1-5. dia)

**1. dia: Címlap**
Az előadás címe: **"Adatvezérelt rendszerek: Lekérdezés optimalizálás"**. A fókuszban az áll, hogyan gondolkodik egy adatbázis-kezelő rendszer, és hogyan teszi a lekérdezéseinket a lehető leggyorsabbá.

**3-5. dia: Mi a lekérdezés optimalizálás célja?**
*   **A hardvereszközök:** A dia képei (merevlemez, RAM, CPU) azt szimbolizálják, hogy a lekérdezés végrehajtása ezeket az erőforrásokat használja.
*   **A cél:** Minimalizálni ezeknek az erőforrásoknak a használatát, és ezáltal a lekérdezés **válaszidejét** (a stopperóra a 4. dián).
*   **Befolyásoló tényezők (5. dia):**
    1.  **I/O költség (Lemezművelet):** Ez a **legmeghatározóbb tényező!** A lemezről olvasni nagyságrendekkel (több ezerszer) lassabb, mint a memóriából. Az optimalizálás fő célja a lemezműveletek számának csökkentése.
    2.  **CPU használat:** Összetett számítások, rendezések, aggregálások terhelik a processzort.
    3.  **Memória használat:** Nagy köztes eredményhalmazok tárolása, `JOIN` műveletek pufferei foglalják a memóriát. A cache (gyorsítótár) hatása itt jelentős: ha a szükséges adatok már a memóriában vannak, az I/O költség drasztikusan csökken.

---

### Az Optimalizálás Története és Alapelvei (6-9. dia)

**6. dia: Rövid történelem**
A lekérdezés-optimalizálás az adatbázis-kezelők evolúciójának központi eleme volt.
*   **'70-es évek:** Az SQL deklaratív jellegének (megmondom, *mit* akarok, nem azt, hogy *hogyan*) megjelenésével a "hogyan" kitalálása a rendszer feladata lett.
*   **Heurisztikus optimalizálás:** Korai, szabályalapú próbálkozások.
*   **'80-as, '90-es évek:** Megjelent a **költségalapú optimalizálás (Cost-Based Optimization - CBO)**. A rendszer több lehetséges végrehajtási tervet generál, megbecsüli mindegyik "költségét" (I/O, CPU), és a legolcsóbbat választja.

**7. dia: Optimalizálás alapelvek**
Hogyan működik a modern költségalapú optimalizáló?
1.  **Statisztikák alapján értékel:** Az optimalizáló **statisztikákat** gyűjt és tart karban a táblákról: sorok száma, az értékek eloszlása az oszlopokban (hisztogramok) stb. Ezek alapján tudja megbecsülni, hogy egy adott szűrési feltétel (pl. `WHERE város = 'Budapest'`) hány sort fog visszaadni. A **költség** egy szám, ami a becsült válaszidőt reprezentálja (CPU + I/O).
2.  **Triviális terv:** Nagyon egyszerű lekérdezésekre (pl. `SELECT * FROM tábla WHERE id = 5`) nem fut le a teljes, bonyolult optimalizálási folyamat, mert csak egyetlen ésszerű végrehajtási mód létezik.
3.  **Ha nem triviális:** Összetett `JOIN`-okat tartalmazó lekérdezések esetén elindul a többfázisú optimalizálás.

**8. dia: Háromfázisú optimalizáció (MS SQL-ben)**
Az optimalizáló nem vizsgálja meg az összes lehetséges (akár milliárdnyi) tervet, mert az túl sokáig tartana. Ehelyett fázisokban dolgozik:
*   **0. Fázis:** Gyors, egyszerű átalakításokat végez. Ha talál egy "elég jó" tervet, aminek a költsége egy alacsony küszöbérték (X) alatt van, akkor azt azonnal végrehajtja.
*   **1. Fázis:** Ha az előző fázis nem talált jó tervet, az optimalizáló több időt szán a keresésre, bonyolultabb átalakításokat is megvizsgál. Ha talál egy tervet egy magasabb küszöbérték (Y) alatt, azt választja.
*   **2. Fázis:** Ha még mindig nincs terv, az optimalizáló a párhuzamos végrehajtás lehetőségét is megvizsgálja, ami a legköltségesebb fázis.

**9. dia: A lekérdezés-feldolgozás teljes menete**
Ez egy kulcsfontosságú ábra, ami a teljes folyamatot mutatja.
1.  **Elemző (Parser):**
    *   *Fordítás:* Leellenőrzi az SQL szintaktikáját.
    *   *Logikai terv készítése:* Létrehoz egy **logikai tervet**, ami egy fa-struktúra a relációs algebra műveleteiből (lásd később). Ez leírja, hogy *mit* kell csinálni, de azt nem, hogy *hogyan*.
2.  **Optimalizáló (Optimizer):** Ez a rendszer "agya".
    *   A logikai tervet átalakítja több, ekvivalens logikai tervvé.
    *   A statisztikák alapján megbecsüli mindegyik költségét.
    *   Kiválasztja a legalacsonyabb költségű tervet, és létrehozza a **fizikai tervet**. Ez már konkrét algoritmusokat (pl. Hash Join, Index Scan) tartalmaz.
3.  **Sorfordító (Row Source Generator):** A fizikai tervet egy végrehajtható kóddá alakítja.
4.  **Végrehajtó (Execution Engine):** Lefuttatja a kódot és visszaadja az eredményt.

---

### A Logikai Terv (10-17. dia)

**11. dia: A logikai végrehajtási terv elemei**
*   **Elemző fa:** Egy fa-struktúra, ahol a levelek a táblák (relációk), a csomópontok pedig a rajtuk végzett műveletek. Az adatok lentről felfelé "áramlanak".
*   **Relációs algebra műveletei:** Az SQL nyelv matematikai alapjai.
    *   **Szelekció (σ):** Sorok szűrése (`WHERE` feltétel).
    *   **Projekció (π):** Oszlopok kiválasztása (`SELECT oszloplista`).
    *   **Összekapcsolás (⋈):** Táblák összekapcsolása (`JOIN`).
    *   **Descartes-szorzat (×):** Minden sort minden sorral összekapcsol. A modern adatbázisok kerülik.

**12. dia: Egyszerű lekérdezés elemző fája**
A dia bemutatja, hogy egy egyszerű SQL lekérdezésnek is több, logikailag ekvivalens terve lehet.

```sql
SELECT p.name
FROM Product p JOIN Category c ON p.CategoryID = c.ID
WHERE c.Name = 'LEGO'
```
*   **Bal oldali terv (rosszabb):** Először összekapcsolja a teljes `Product` és `Category` táblát, és csak a hatalmas eredményhalmazon végez szűrést a 'LEGO' névre.
*   **Jobb oldali terv (jobb):** Először leszűri a `Category` táblát a 'LEGO'-ra (ami valószínűleg csak egyetlen sort ad vissza), és utána csak ezt az egy sort kapcsolja össze a `Product` táblával.

**13-15. dia: Ekvivalens átalakítások**
Az optimalizáló ezeket a matematikai azonosságokat használja a tervek átalakítására. A legfontosabb heurisztika (ökölszabály): **"Told le a szelekciót és a projekciót a fa legaljára, amilyen mélyre csak lehet!"**
*   Ez azt jelenti, hogy a szűrést (`WHERE`) és az oszlopok kiszórását (`SELECT p.name`) a lehető leghamarabb kell elvégezni, hogy a `JOIN` műveleteknek már kisebb táblákkal kelljen dolgozniuk.

**17. dia: Heurisztika: Szabályok**
Gyakorlati tanácsok, amik segítik az optimalizálót:
*   **Szűrjünk minél előbb:** A `WHERE` feltételeket a lehető legkorábban alkalmazzuk.
*   **Projektáljunk minél előbb:** Csak azokat az oszlopokat kérjük le, amikre tényleg szükségünk van. **Kerüljük a `SELECT *`-ot!**
*   **A legerősebb szűréssel kezdjünk:** Azt a feltételt tegyük előre, ami a legtöbb sort kiszűri.
*   **A legszűkebb joinokkal kezdjünk:** Először azokat a táblákat kapcsoljuk össze, amik a legkisebb köztes eredményt adják.

---

### A Fizikai Terv (18-33. dia)

A fizikai terv már konkrét algoritmusokat (operátorokat) rendel a logikai műveletekhez.

**19. dia: Táblaelérési módok**
Hogyan olvassa be a rendszer egy tábla adatait?
1.  **Table Scan (Teljes átvizsgálás):** Végigolvassa a tábla összes sorát. Akkor használja, ha nincs használható index, vagy ha a tábla sorainak nagy részére szükség van.
2.  **Index alapú átvizsgálás:** Egy index segítségével hatékonyabban találja meg a szükséges sorokat.

**20-22. dia: Indexelt táblaelérési módok (MS SQL)**
*   **Index Scan:** Végigolvassa az index *összes* levélelemét. Ez akkor hatékony, ha az index kisebb, mint a tábla maga.
*   **Index Seek (Keresés):** **Ez a leghatékonyabb!** Az index fa-struktúráját használva, "fatörzsön lefelé lépkedve" (logaritmikus időben) közvetlenül ráugrik a keresett értékre. `WHERE ID = 123` vagy `WHERE Ár > 1000` típusú lekérdezések használják.
*   **Clustered vs. Non-clustered Index:**
    *   **Clustered Index:** Maguk az adatsorok a lemezen fizikailag az index kulcsa szerint vannak sorba rendezve. Egy táblának csak egy ilyen lehet. Az elsődleges kulcs általában clustered.
    *   **Non-clustered Index:** Egy különálló struktúra, ami a kulcsérték mellett egy "mutatót" (bookmark/RID) tárol a tényleges adatsor fizikai helyére. Egy táblának több ilyen is lehet.

**24-27. dia: Join operátorok megvalósítási módjai**
Hogyan kapcsol össze az adatbázis két táblát?
1.  **Nested Loop Join (Egymásba ágyazott ciklus):** A legegyszerűbb. Végigmegy a külső tábla minden során, és minden sorhoz végigmegy a belső tábla összes során, hogy párokat keressen. Nagyon lassú (`O(N*M)`), csak akkor hatékony, ha az egyik tábla nagyon kicsi.
2.  **Hash Join:** Rendkívül hatékony nagy, nem rendezett táblák összekapcsolására.
    *   *Build fázis:* A kisebbik táblából egy hash táblát épít a memóriában a join kulcs alapján.
    *   *Probe fázis:* Végigmegy a nagyobbik táblán, és minden sor join kulcsát "megkeresi" a hash táblában.
3.  **Merge Join (Összefésülő join):** Akkor a leghatékonyabb, ha mindkét tábla már eleve rendezve van a join kulcs szerint. A két rendezett listát "összefésüli", mint egy cipzárt.

**28-31. dia: A végrehajtási terv megtekintése (MS SQL)**
Az SQL Server Management Studio-ban (SSMS) a `Ctrl+L` vagy `Ctrl+M` lenyomásával meg tudjuk jeleníteni egy lekérdezés **grafikus végrehajtási tervét**.
*   Ez egy fa diagram, ami jobbról balra olvasandó.
*   Minden ikon egy **fizikai operátort** jelöl (pl. Clustered Index Seek, Nested Loops).
*   Az ikonok fölé víve az egeret részletes információkat kapunk: a művelet becsült költsége, a sorok becsült és tényleges száma.
*   A nyilak vastagsága arányos a rajtuk áthaladó sorok számával.
*   **A cél:** A legdrágább (legnagyobb százalékos költségű) operátorok megkeresése és optimalizálása, pl. egy hiányzó index létrehozásával (az SSMS erre gyakran javaslatot is tesz!).

**32-33. dia: Plan Cache és Önhangolás**
*   **Plan Cache:** A szerver a legenerált végrehajtási terveket egy gyorsítótárban tárolja. Ha ugyanaz a lekérdezés (vagy szerkezetileg hasonló) érkezik újra, nem kell újraoptimalizálni, csak előveszi a kész tervet a cache-ből.
*   **Önhangolás (Adaptive Query Processing):** A modern adatbázis-kezelők futás közben is figyelik a tervek teljesítményét. Ha a statisztikák elavultak és a becsült sorszámok nagyon eltérnek a valóságtól, a rendszer képes menet közben is tervet váltani, vagy frissíteni a statisztikákat.

---

### Jó Tanácsok és MongoDB Optimalizálás (34-49. dia)

**34-38. dia: Jó tanácsok (SQL)**
Összefoglaló a hatékony SQL íráshoz:
*   **Statisztikák legyenek naprakészek!**
*   **Kerüld a `SELECT *`-ot!**
*   **Használj `JOIN`-t** az `IN`, `NOT IN`, `EXISTS` helyett, ahol lehet (a modern optimalizálók ezt gyakran megteszik helyettünk).
*   **`UNION ALL` gyorsabb**, mint a `UNION`, mert nem kell a duplikátumokat kiszűrnie.
*   **`WHERE` feltételben ne használj függvényt a kulcs oszlopon!**
    *   `WHERE YEAR(Datum) = 2025` → **ROSSZ**, mert nem tudja használni a `Datum`-ra tett indexet.
    *   `WHERE Datum >= '2025-01-01' AND Datum < '2026-01-01'` → **JÓ**, mert a kifejezés "SARGable" (Search Argument Able), azaz az optimalizáló tudja használni az indexet a tartomány kereséséhez.

**39-46. dia: MongoDB Indexek és Optimalizálás**
A MongoDB optimalizálója egyszerűbb, mint a relációs társaié, de az alapelvek hasonlók.
*   **Nincsenek statisztikák:** A MongoDB nem használ részletes statisztikákat.
*   **Több terv versenyeztetése:** Egy lekérdezéshez több lehetséges tervet is generál (ha több index is használható), elkezdi párhuzamosan futtatni őket, és azt választja, amelyik a leggyorsabban adja vissza az első néhány eredményt.
*   **Query Shape és Terv Cache:** A MongoDB is gyorsítótárazza a terveket a lekérdezés "alakja" (`query shape`) alapján, ami a szűrési és rendezési mezőket tartalmazza, de a konkrét értékeket nem.
*   **Optimalizációs lépések:** A MongoDB is megpróbálja a szűréseket (`$match`) és a lapozást (`$skip`, `$limit`) a pipeline elejére tolni.
*   **`explain()`:** A MongoDB-ben a `.explain()` metódussal lehet lekérni egy lekérdezés végrehajtási tervét. A legfontosabb "stage"-ek (lépések):
    *   **`COLLSCAN` (Collection Scan):** Teljes gyűjtemény-végigolvasás. **A legrosszabb, ezt kell elkerülni!**
    *   **`IXSCAN` (Index Scan):** Index használata a dokumentumok megtalálására. **Ez a cél!**
    *   **`FETCH`:** Az `IXSCAN` utáni lépés, ami az index által megtalált mutatók alapján beolvassa a teljes dokumentumokat.