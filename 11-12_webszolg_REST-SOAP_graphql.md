Rendben, vágjunk is bele az utolsó, a webszolgáltatásokról szóló diasorba, a megszokott részletességgel és alapossággal. Ez az anyag arról szól, hogyan kommunikálnak egymással az elosztott rendszerek komponensei a hálózaton keresztül.

---

### Bevezetés: Szolgáltatásorientált Architektúra (SOA) (1-8. dia)

**1. dia: Címlap**
Az előadás címe: **"Adatvezérelt rendszerek: Webszolgáltatások és REST, GraphQL"**. A témakör a modern alkalmazások közötti kommunikáció három kulcsfontosságú technológiája.

**2-3. dia: Szolgáltatás fogalma**
A "szolgáltatás" a háromrétegű architektúrából nőtt ki. Ahelyett, hogy egy monolitikus "üzleti logikai rétegről" beszélnénk, ezt a réteget kisebb, önálló, újrafelhasználható egységekre, **szolgáltatásokra** bontjuk.
*   **Fókusz:** A hangsúly a szolgáltatás **interfészére** (Service Interface) kerül. Ez a "szerződés" (contract), ami leírja, hogy a szolgáltatás milyen műveleteket kínál a külvilág felé.
*   **Új szempontok:** A szolgáltatás-alapú gondolkodás az **elosztott** és **heterogén** (eltérő technológiájú, pl. Java és .NET) rendszerek együttműködésének igényéből fakadt.

**4. dia: Service Oriented Architecture - SOA**
*   **Definíció:** A SOA egy architekturális stílus, ahol az alkalmazásokat egymással lazán csatolt, szabványos interfészeken keresztül kommunikáló **szolgáltatásokból** építjük fel.
*   **Cél:** A monolitikus "alkalmazásszigetek" helyett egy rugalmas, újrafelhasználható komponensekből álló rendszert létrehozni, ami képes különböző platformok (Java, .NET, Ruby stb.) között is működni.

**5. dia: SOA példa**
Az ábra tökéletesen szemlélteti a SOA lényegét:
*   Vannak különböző kliensek (`Desktop`, `Web`, `Mobile`), amik különböző technológiával készültek.
*   Vannak különböző, önálló üzleti funkciókat ellátó backend szolgáltatások (`Customer Service`, `Order Service`, `Payment Service`), amik szintén különböző technológiákkal készültek.
*   Mindannyian egy központi `Order Service`-szel kommunikálnak egy szabványos protokollon keresztül, anélkül, hogy tudniuk kellene egymás belső működéséről.

**6-7. dia: (Web)szolgáltatás fogalma összefoglalva**
Egy webszolgáltatás egy olyan szoftverkomponens, ami:
1.  **Műveleteket publikál** egy interfészen keresztül (pl. `createOrder`, `getOrderStatus`).
2.  **Hálózaton**, szabványos protokollokkal (pl. HTTP) kommunikál üzenetekkel.
3.  **Független egység:** Önállóan telepíthető, verziózható és fejleszthető.
4.  **Platformfüggetlen:** Bármilyen technológiával megvalósítható.

**8. dia: SOA megvalósítások: SOAP és REST**
A SOA elveinek megvalósítására két fő megközelítés terjedt el:
1.  **SOAP (Simple Object Access Protocol):** Egy régebbi, szigorúan szabványosított, XML-alapú **protokoll**.
2.  **REST (Representational State Transfer):** Egy modernebb, könnyebb súlyú **architekturális stílus**, ami a web (HTTP) meglévő protokolljaira épül.

A **GraphQL** egy még újabb alternatíva, ami a REST néhány problémájára ad választ.

---

### II. Rész: SOAP Webszolgáltatások (9-17. dia)

**9. dia:** Elválasztó dia.

**10. dia: SOAP jellemzői**
*   **Protokoll:** Szigorú szabályrendszer az üzenetek formátumára és a kommunikációra.
*   **XML alapú:** A kérések és válaszok is XML dokumentumok.
*   **WSDL (Web Services Description Language):** A szolgáltatás interfészét egy szabványos, XML-alapú leírónyelven, a WSDL-ben kell definiálni. Ez a "szerződés" a kliens és a szerver között.
*   **WS\* szabványok:** Rengeteg kiegészítő szabvány ("WS-csillag") létezik hozzá, pl. `WS-Security` (biztonság), `WS-AtomicTransaction` (elosztott tranzakciók), ami nagyon robusztussá, de egyben nagyon bonyolulttá is teszi.
*   **Komplexitás:** Nehézkes, "nehézsúlyú" (heavyweight) technológia.

**11-13. dia: SOAP üzenet**
*   Egy SOAP üzenet egy `Envelope` (boríték), amiben van egy `Header` (fejléc, opcionális, meta-információknak) és egy `Body` (törzs).
*   A `Body`-ban található a konkrét metódushívás (pl. `<m:GetStockPrice>`) és annak paraméterei.
*   **Fontos:** Ezeket az XML-eket **sosem írjuk kézzel!** A fejlesztői környezetek (pl. .NET WCF, Java JAX-WS) a programkódból (interfészekből és osztályokból) automatikusan generálják a SOAP üzeneteket, és a bejövő üzeneteket visszaalakítják metódushívássá. A fejlesztő csak annyit lát, hogy meghív egy távoli metódust:

    ```csharp
    // A fejlesztő számára a hálózati kommunikáció rejtve marad.
    decimal price = stockService.GetStockPrice("IBM");
    ```

**14-16. dia: WSDL szerepe**
*   A WSDL a SOAP szolgáltatások **önleíró** mechanizmusa. Ez egy gép által feldolgozható XML, ami pontosan leírja a szolgáltatás összes műveletét, azok paramétereit és a visszatérési értékek típusait.
*   **Tipikus folyamat:**
    1.  **Szerver oldal:** A fejlesztő megírja a szolgáltatás logikáját (pl. egy C# interfészt és osztályt). A keretrendszer ebből **generál egy WSDL fájlt**.
    2.  **Kliens oldal:** A kliens fejlesztője fogja ezt a WSDL fájlt, és egy eszközzel (pl. `svcutil.exe`) **generáltat belőle egy kliens oldali proxy osztályt**.
    *   Ez a proxy osztály fogja tartalmazni azokat a metódusokat (pl. `GetStockPrice`), amiket a kliens meghívhat. A proxy feladata a hívás becsomagolása SOAP üzenetbe, elküldése, és a válasz kicsomagolása.

**17. dia: SOAP hibakezelés**
A SOAP szabványos hibakezelési mechanizmust definiál a **SOAP Fault** segítségével. Ez egy speciális, hibát leíró válaszüzenet. A modern keretrendszerek ezt automatikusan lekezelik, és a kliens oldalon egy standard **kivétellé (exception)** alakítják át.

---

### III. Rész: REST(ful) Szolgáltatások (18-34. dia)

**18. dia:** Elválasztó dia.

**19. dia: REST - Representational State Transfer**
*   **Architektúra stílus, nem protokoll:** A REST egy sor alapelvet és megkötést definiál, nem egy konkrét szabvány, mint a SOAP.
*   **HTTP-re épül:** Ahelyett, hogy új protokollt találna ki, a web alapját adó HTTP protokollt használja, annak minden lehetőségével.
*   **Alapelv: Erőforrások (Resources):** A REST-ben nem műveletekben, hanem **erőforrásokban** gondolkodunk. Egy erőforrás bármi lehet, aminek van egyedi azonosítója (pl. egy felhasználó, egy termék, egy rendelés).
*   **Címzés URL-lel:** Minden erőforrás egyedi címmel (URL) rendelkezik. Pl. `GET /api/todos/12` lekéri a 12-es azonosítójú "todo" elemet.
*   **HTTP igék szemantikája:** A műveleteket a szabványos HTTP metódusokkal ("igékkel") fejezzük ki.
*   **Állapotmentesség (Stateless):** A szerver nem tárolja a kliens állapotát a kérések között. Minden kérésnek tartalmaznia kell az összes információt, ami a végrehajtásához szükséges. Ez teszi a REST szolgáltatásokat rendkívül jól skálázhatóvá.

**20-22. dia: REST URL és HTTP igék kapcsolata (CRUD)**
Ez a REST legfontosabb koncepciója. A CRUD (Create, Read, Update, Delete) műveletek leképezése az erőforrásokra és a HTTP igékre:

| Művelet | HTTP Ige | URL Példa | Leírás | Idempotens? |
| :--- | :--- | :--- | :--- | :--- |
| **Listázás** | `GET` | `/tasks` | Visszaadja az összes "task" erőforrást. | Igen |
| **Lekérés** | `GET` | `/tasks/23` | Visszaadja a 23-as azonosítójú "task"-ot. | Igen |
| **Létrehozás** | `POST` | `/tasks` | Létrehoz egy új "task"-ot a kérés törzsében (body) küldött adatok alapján. | **Nem** |
| **Teljes frissítés** | `PUT` | `/tasks/23` | Lecseréli a 23-as "task"-ot a kérés törzsében küldött adatokra. | Igen |
| **Részleges frissítés** | `PATCH` | `/tasks/23` | Módosítja a 23-as "task" megadott mezőit. | **Nem** |
| **Törlés** | `DELETE` | `/tasks/23` | Törli a 23-as azonosítójú "task"-ot. | Igen |

*   **Idempotens:** Egy művelet idempotens, ha akárhányszor is hajtjuk végre ugyanazokkal a paraméterekkel, a rendszer állapota ugyanaz lesz, mint az első végrehajtás után.

**23-28. dia: REST a gyakorlatban**
*   **Adatküldés:** Paramétereket az URL-ben (path vagy query string), a HTTP fejlécekben, vagy a kérés törzsében (body) lehet küldeni. A szűrést, rendezést tipikusan a query stringben adjuk meg (`/tasks?sort=-priority`).
*   **Hibakezelés:** Nincs szabványos hibatörzs, de a gyakorlat a megfelelő **HTTP státuszkódok** használata (pl. `404 Not Found`, `500 Internal Server Error`) és egy JSON objektum visszaadása a hiba leírásával.
*   **Biztonság:** Mindig `HTTPS`-t kell használni. Az authentikáció leggyakrabban valamilyen **token-alapú** megoldással történik (pl. JWT - JSON Web Token), amit a kliens minden kérés `Authorization` fejlécében elküld.

**29-34. dia: REST vs. SOAP és a "mennyire REST?" kérdés**
*   **REST előnyei:** Könnyűsúlyú, egyszerű, jobban skálázható, minden platform (főleg a böngészők és mobilok) natívan "érti". Ma már ez a domináns megközelítés.
*   **SOAP előnyei:** Szabványosított, beépített támogatás a komplex vállalati funkciókhoz (pl. tranzakciók, biztonsági policyk).
*   **REST-szemlélet:** Tiszta REST-ben **erőforrásokban** gondolkodunk, nem műveletekben.
    *   **Rossz:** `GET /API/books/delete?id=12` (A `delete` egy művelet, nem erőforrás).
    *   **Jó:** `DELETE /API/books/12`
*   **Pragmatikus REST (Web API):** A gyakorlatban sokszor nehéz minden logikát a tiszta CRUD sémára illeszteni. Ilyenkor előfordulnak "művelet-szerű" URL-ek (pl. `POST /collections/entries/move`). Ha egy API a HTTP eszköztárát használja, de nem követi szigorúan az erőforrás-központú szemléletet, gyakran **Web API**-nak vagy **HTTP API**-nak hívják.

---

### IV. Rész: .NET Web API és Dokumentáció (35-57. dia)

**35-47. dia: .NET Core Web API Példa**
Ez a rész egy konkrét .NET Core implementációt mutat be a REST alapelvek mentén.
*   **`ControllerBase`:** A Web API controllerek ebből az ősosztályból származnak.
*   **Routing:** Attribútumokkal (`[Route]`, `[HttpGet]`, `[HttpPost]`) kötjük össze az URL-eket és HTTP igéket a controller metódusaival (`Action`).

```csharp
// Ez a controller az "api/todos" útvonal alatti kéréseket fogja kezelni.
[Route("api/[controller]")]
[ApiController]
public class TodosController : ControllerBase
{
    // GET /api/todos/{id} kérésre fog lefutni.
    [HttpGet("{id}")]
    public ActionResult<TodoItem> GetById(long id)
    {
        var item = _context.TodoItems.Find(id);
        if (item == null)
        {
            // Visszaad egy standard 404 Not Found választ.
            return NotFound();
        }
        // Visszaad egy 200 OK választ, a törzsben a JSON-né szerializált 'item' objektummal.
        return item;
    }

    // POST /api/todos kérésre fog lefutni.
    [HttpPost]
    public IActionResult Create([FromBody] TodoItem item)
    {
        _context.TodoItems.Add(item);
        _context.SaveChanges();

        // Visszaad egy 201 Created választ. A 'Location' fejlécben benne lesz az új erőforrás URL-je.
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }
}
```

**51-57. dia: REST API Dokumentáció (OpenAPI/Swagger)**
*   **Probléma:** Honnan tudja a kliens fejlesztője, hogy milyen végpontok léteznek, és azokat hogyan kell használni?
*   **Megoldás: OpenAPI (régi nevén Swagger):** Egy specifikáció a REST API-k leírására. Olyan a REST világában, mint a WSDL a SOAP világában.
*   **Swagger Eszköztár:**
    *   **SwaggerGen:** Könyvtár, ami a .NET Web API controller kódból (az attribútumok és típusok alapján) automatikusan **legenerálja az OpenAPI leíró fájlt (swagger.json)**.
    *   **SwaggerUI:** Egy webes felület, ami ebből a JSON-ből egy interaktív, ember által olvasható **HTML dokumentációt generál**, ahol az egyes végpontokat ki is lehet próbálni.
*   .NET-ben a `Swashbuckle.AspNetCore` NuGet csomaggal lehet ezt a funkcionalitást egyszerűen integrálni.

---

### V. Rész: GraphQL (58-76. dia)

**58-61. dia: Mi a GraphQL?**
A GraphQL egy újabb API **lekérdező nyelv** és futtatókörnyezet, amit a Facebook fejlesztett ki a REST néhány problémájának megoldására.
*   **Nem adatbázis technológia!** Ez is egy API publikálási forma, ami bármilyen backend fölé helyezhető.
*   **Deklaratív:** A kliens **pontosan megmondja, milyen adatokra van szüksége**, beleértve a beágyazott/kapcsolódó adatokat is.

**62-64. dia: A REST problémái, amiket a GraphQL megold**
1.  **Over-fetching (túltöltés):** A REST végpontok fix adatstruktúrát adnak vissza. Ha a kliensnek csak egy mezőre van szüksége, a szerver akkor is elküldi az összeset, felesleges hálózati forgalmat generálva.
2.  **Under-fetching (alultöltés):** Ha a kliensnek kapcsolódó adatokra is szüksége van (pl. egy felhasználó és annak posztjai), több külön kérést kell indítania a szerver felé (`/users/1`, majd `/users/1/posts`), ami lassú.

**A GraphQL megoldása:**
*   Egyetlen végpont van (jellemzően `/graphql`).
*   A kliens egyetlen `POST` kérésben elküld egy **lekérdezés-dokumentumot**, ami pontosan leírja a szükséges adatstruktúrát.
*   A szerver pontosan ebben a struktúrában adja vissza a választ, egyetlen körben.

**65-69. dia: GraphQL séma és lekérdezések**
*   **Erősen Típusos Séma:** A GraphQL egy erős típusrendszerre épül. A szolgáltatás összes elérhető típusát és mezőjét egy **sémában** kell definiálni (GraphQL Schema Definition Language - SDL). Ez a "szerződés" a kliens és a szerver között.
*   A `Query` típus definiálja a belépési pontokat (lekérdezéseket).

```graphql
# Séma definíció (SDL)
type Query {
  # Egy lekérdezés, ami visszaadja az összes Person-t, opcionális 'last' paraméterrel.
  allPersons(last: Int): [Person!]!
}
type Person {
  name: String!
  age: Int
  posts: [Post!]!
}

# Kliens oldali lekérdezés
{
  allPersons(last: 2) {
    name
    posts {
      title
    }
  }
}
```

**70-76. dia: Műveletek és Értékelés**
*   **Művelet típusok:**
    1.  **Query:** Adatok lekérdezése (csak olvasás).
    2.  **Mutation:** Adatok módosítása (írás: create, update, delete).
    3.  **Subscription:** Valós idejű adatfrissítések fogadása (WebSocketen keresztül).
*   **Előnyök:** Hatékony (nincs over/under-fetching), rugalmas, erősen típusos.
*   **Hátrányok:** A szerver oldali logika bonyolultabb (védekezni kell a túl komplex lekérdezések ellen), a HTTP cache-elés nehezebb, és az ökoszisztéma még mindig kevésbé érett, mint a REST-é.