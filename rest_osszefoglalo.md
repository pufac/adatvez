Ez egy kiváló kérdés, mert a REST API-k lelke pontosan ez: hogyan kombináljuk a **HTTP igéket (Verbs)** és az **URL mintákat**.

A vizsgán és a gyakorlatban alapvetően **három helyről** szedhetünk ki adatot, és ezek határozzák meg, hogy néz ki az URL.

Itt van a "Szentháromság", amit tudnod kell:

---

### 1. Path Parameter (Útvonal változó) – "MELYIKET?"
Ezt akkor használjuk, ha egy **konkrét** elemet akarunk azonosítani (pl. az 5-ös ID-jú tantárgyat). Ez az URL része.

*   **URL példa:** `GET /subjects/5`
*   **Mikor használjuk?** Ha egy konkrét erőforrást akarunk elérni az ID-ja alapján.
*   **C# kód:** `[FromRoute]` (vagy simán az argumentum neve alapján felismeri).
*   **Jelölés kódban:** `[HttpGet("{id}")]`

### 2. Query Parameter (Lekérdezési paraméter) – "HOGYAN / MILYET?"
Ez a kérdőjel `?` utáni rész. Ez **opcionális** szűrésre, keresésre, lapozásra való. Nem változtatja meg, *melyik* végpontot hívod meg, csak finomhangolja az eredményt.

*   **URL példa:** `GET /subjects?search=matek&minCredit=2`
*   **Mikor használjuk?** Keresés, szűrés, rendezés.
*   **C# kód:** `[FromQuery]`

### 3. Request Body (Kérés törzse) – "MIT?"
Ez nem látszik az URL-ben. Itt küldjük a bonyolult adatokat (JSON).

*   **URL példa:** `POST /subjects` (az adat "láthatatlanul" utazik).
*   **Mikor használjuk?** Létrehozásnál, módosításnál.
*   **C# kód:** `[FromBody]`

---

### A REST Nagy Táblázata (Cheat Sheet)

Tegyük fel, hogy **Termékeket (`products`)** kezelünk. Így néznek ki a szabványos REST kérések:

| HTTP Ige | URL Minta | Mire való? | C# Annotáció | Honnan jön az adat? |
| :--- | :--- | :--- | :--- | :--- |
| **GET** | `/products` | **Összes** listázása | `[HttpGet]` | - |
| **GET** | `/products?color=red` | **Szűrés / Keresés** | `[HttpGet]` | `[FromQuery]` (URL ? után) |
| **GET** | `/products/10` | **Egy konkrét** lekérése (ID: 10) | `[HttpGet("{id}")]` | `[FromRoute]` (URL része) |
| **POST** | `/products` | **Új létrehozása** | `[HttpPost]` | `[FromBody]` (JSON adat) |
| **PUT** | `/products/10` | **Teljes csere** (ID: 10) | `[HttpPut("{id}")]` | `[FromBody]` + `[FromRoute]` |
| **PATCH**| `/products/10` | **Részleges módosítás** | `[HttpPatch("{id}")]` | `[FromBody]` + `[FromRoute]` |
| **DELETE**| `/products/10` | **Törlés** (ID: 10) | `[HttpDelete("{id}")]` | `[FromRoute]` |

---

### Példák részletesen (C# kóddal)

#### 1. Konkrét elem lekérése (Path Parameter)
Itt az `id` az URL része.
**URL:** `GET http://localhost/subjects/5`

```csharp
[HttpGet("subjects/{id}")] // A {id} jelzi, hogy ez az útvonal része
public IActionResult GetSubject(int id) // A keretrendszer tudja, hogy a {id}-t ide kell tenni
{
    // Megkeressük az 5-ös ID-t
    return Ok(subject);
}
```

#### 2. Keresés / Szűrés (Query Parameter)
Ez volt a te példád. Itt a `query` nem az útvonal fix része, hanem opció.
**URL:** `GET http://localhost/subjects/search?query=matematika`

```csharp
[HttpGet("subjects/search")] 
public IActionResult Search([FromQuery] string query) // Explicit megmondjuk: ? után keresd!
{
    // Keresünk mindenkit, akinek a nevében benne van a "query"
    return Ok(results);
}
```

#### 3. Új létrehozása (Body)
Itt az URL egyszerű, az adat rejtett.
**URL:** `POST http://localhost/subjects`
**Body:** `{"name": "Fizika", "credit": 4}`

```csharp
[HttpPost("subjects")]
public IActionResult Create([FromBody] Subject subject) // A JSON-t alakítsd Subject objektummá
{
    // Mentés adatbázisba...
    return CreatedAtAction(...);
}
```

### Összefoglaló a vizsgához:
1.  Ha **ID** alapján hivatkozol valamire (törlés, módosítás, egy darab lekérése) -> Az URL-be tedd (`/products/5`) -> **`[FromRoute]`**.
2.  Ha **keresel**, válogatsz -> A végére tedd kérdőjellel (`?q=alma`) -> **`[FromQuery]`**.
3.  Ha **adatot küldesz** (mentés) -> JSON-be tedd -> **`[FromBody]`**.