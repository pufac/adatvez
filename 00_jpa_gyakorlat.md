Szívesen végigvezetlek ezen az anyagon. Ez egy klasszikus **Java Spring Boot + JPA (Java Persistence API)** laborgyakorlat, ami a modern vállalati szoftverfejlesztés egyik legfontosabb technológiai stackje.

A cél az, hogy egy webes alkalmazás (Spring Boot) képes legyen kommunikálni egy relációs adatbázissal (MS SQL), adatokat írni, olvasni és módosítani anélkül, hogy rengeteg nyers SQL kódot kellene írni. Ezt hívjuk **ORM-nek (Object-Relational Mapping)**.

Nézzük feladatonként, mi történik!

---

### Feladat 0, 1, 2: Előkészületek (Adatbázis és Projekt)

Ezek a "beüzemelési" lépések.
*   **Technika:** **MS SQL Server** a választott adatbázis motor.
*   **Lényeg:** Létrehozol egy `adatvez` nevű adatbázist és lefuttatsz egy `mssql.sql` szkriptet. Ez a szkript hozza létre a táblákat (pl. Products, Users) és tölti fel kezdőadatokkal.
*   **Projekt:** Egy Maven alapú Spring Boot projektet importálsz.
    *   **Maven:** Kezeli a függőségeket (letölti helyetted a Springet, a Hibernate-et, az adatbázis drivert).
    *   **Spring Boot:** A keretrendszer, ami "összedrótozza" az alkalmazást. A `pom.xml`-ben vannak a könyvtárak, az `application.properties`-ben pedig az adatbázis elérési adatai (URL, user, password).

---

### Feladat 3: Entitások áttekintése (ORM alapjai)

Itt találkozol először a **JPA**-val (Java Persistence API).
A lényeg: Java osztályokat feleltetünk meg adatbázis tábláknak.

**Mit kell látni/csinálni?**
A `hu.bme.aut.adatvez.webshop.model` csomagban lévő osztályokat (pl. `Product`, `Category`). Ezek tele vannak `@Annotációkkal`.

**Hogyan működik?**
*   **`@Entity`**: Ez mondja meg a Javanak, hogy ez az osztály (pl. `Product`) egy adatbázis táblát reprezentál.
*   **`@Id`**: Megjelöli, melyik mező az elsődleges kulcs (Primary Key).
*   **`@OneToMany` / `@ManyToOne`**: Ezek a kapcsolatok. Például egy kategóriának sok terméke lehet (OneToMany), de egy termék csak egy kategóriához tartozik (ManyToOne). A JPA ezek alapján tudja, hogyan kezelje a `JOIN`-okat a háttérben.

---

### Feladat 4: Lekérdezések (A legfontosabb rész)

Itt kezdődik a valódi programozás. A cél adatokat kinyerni az adatbázisból különböző módokon. A Spring Data JPA több módszert is kínál erre.

#### 4.a feladat: Egyedi Repository metódus (EntityManager használata)
Itt egy "Custom Repository"-t kell írni. Ez akkor kell, ha a Spring beépített varázslata nem elég, és teljes kontrollt akarsz.

**A kód magyarázata (a képről):**
```java
// Az EntityManager a JPA szíve, ez kezeli az adatbázis kapcsolatot
@PersistenceContext
EntityManager em; 

public List<Product> findByStockGreaterThan(BigDecimal limit) {
    // JPQL (Java Persistence Query Language) lekérdezés írása
    // Figyeld meg: Nem táblaneveket (t_product), hanem Osztályneveket (Product) használ!
    return em.createQuery("SELECT p FROM Product p WHERE p.stock > :limit", Product.class)
             .setParameter("limit", limit) // Paraméter átadása biztonságosan (SQL injection ellen)
             .getResultList(); // Lista visszaadása
}
```
*   **Technika:** **JPQL**. Ez olyan, mint az SQL, de objektumokon dolgozik.
*   **Működés:** A Java kód lefordítja ezt a JPQL-t valódi SQL-re (pl. `SELECT * FROM Products WHERE stock > 10`), lefuttatja, és az eredmény sorokat átalakítja `Product` objektumokká.

#### 4.b feladat: Komplexebb lekérdezés (JOIN)
Itt olyan termékeket keresünk, amiket legalább kétszer rendeltek meg.

**A kód magyarázata:**
```java
public List<Product> findProductsOrderedAtLeastTwice() {
    // Natív SQL vagy komplex JPQL helyett itt a criteria builder vagy JPQL mehet
    return em.createQuery(
        "SELECT p FROM Product p " +
        "LEFT JOIN FETCH p.orderItems " + // Kapcsolt adatok betöltése (EAGER fetch)
        "WHERE size(p.orderItems) >= 2", // Feltétel: legalább 2 rendelés
        Product.class)
        .getResultList();
}
```
*   **Technika:** Itt a `FETCH` kulcsszó fontos. Ez azt mondja a JPA-nak, hogy ne csak a Terméket hozza le, hanem azonnal hozza le a hozzá tartozó rendelési tételeket is egyetlen lekérdezéssel (optimalizálás).

#### 4.c feladat: Named Query (Előre megírt lekérdezés)
Ahelyett, hogy a Java kódban gépelnéd a Query-t stringként, kiteheted az Entitás osztály tetejére annotációba.

**A kód (Product.java tetején):**
```java
@NamedQueries({
    @NamedQuery(name="Product.findAll", query="SELECT p FROM Product p"),
    @NamedQuery(name="Product.findMostExpensive", query="SELECT p FROM Product p WHERE p.price = (SELECT MAX(p2.price) FROM Product p2)")
})
```
**A meghívás (Repository-ban):**
```java
public List<Product> findMostExpensive() {
    return em.createNamedQuery("Product.findMostExpensive", Product.class)
             .getResultList();
}
```
*   **Előny:** A lekérdezés fordítási időben ellenőrizhető (néha), és tisztább a kód.

---

### Feladat 5: Adatmódosítás (UPDATE)

Nemcsak olvasni, írni is kell. Itt tömeges módosítást végzünk.

#### 5.a feladat: @Modifying és @Query
Emeljük meg a "Drága" kategóriájú termékek árát!

**A kód (Repository interfészben):**
```java
@Modifying // Jelzi a Springnek, hogy ez NEM SELECT, hanem UPDATE/DELETE
@Transactional // Tranzakcióba csomagolja (ha hiba van, visszavonja)
@Query("UPDATE Product p SET p.price = p.price * 1.1 WHERE p.id IN " +
       "(SELECT p2.id FROM Product p2 WHERE p2.category.name = :categoryName)")
void categoryRaisePrice(@Param("categoryName") String categoryName);
```
*   **Hogyan működik?** Ez közvetlenül az adatbázisban fut le. Nem tölti be a Java memóriába az összes terméket, nem módosítja őket egyesével, hanem kiküldi az `UPDATE` parancsot a szervernek. Ez sokkal gyorsabb nagy adatmennyiségnél.

#### 5.b és 5.c feladat: Egyszerűsítés Spring Data-val
A 5.b részben láthatod, hogy a kódot be kell kötni a Controllerbe (hogy a böngészőből hívható legyen).

Az 5.c részben (utolsó screenshot alja) látszik a **Spring Data JPA** igazi ereje:
```java
// Csak egy interfészt hozol létre!
public interface CategoryRepository extends JpaRepository<Category, Long> {
    // MAGIC: A metódus NEVE alapján a Spring legenerálja a Query-t!
    // "Find by Name" -> SELECT * FROM Category WHERE name = ?
    Category findByName(String name);
}
```
*   **Technika:** **Query Methods**. Nem kell SQL-t írnod, csak betartani a névadási konvenciót (`findBy...`), és a Spring megcsinálja a lekérdezést a háttérben.

---

### Feladat 6: Tárolt eljárások (Stored Procedures)

Néha a logika az adatbázisban van (SQL nyelven megírva), nem Javaban. Ezt hívjuk tárolt eljárásnak.

**1. Lépés:** Létrehozni az eljárást az adatbázisban (SQL kód a képen: `CREATE PROCEDURE ...`).
**2. Lépés:** Leképezni Javanál.

**A kód (JPA Entitáson):**
```java
@NamedStoredProcedureQuery(
    name = "createMethodSP", // Ezzel a névvel hivatkozunk rá Javaban
    procedureName = "CreateNewPaymentMethod", // Ez a neve az SQL szerveren
    parameters = {
        @StoredProcedureParameter(mode = ParameterMode.IN, name="Method", type=String.class),
        // ... többi paraméter
    }
)
@Entity
public class PaymentMethod ...
```
**A meghívás (Repository-ban):**
```java
public void createNewMethod(String method, int deadline) {
    StoredProcedureQuery sp = em.createNamedStoredProcedureQuery("createMethodSP");
    sp.setParameter("Method", method);
    sp.setParameter("Deadline", deadline);
    sp.execute(); // Lefuttatja az eljárást az adatbázisban
}
```

---

### Összefoglalva a lényeg vizsgára:

1.  **JPA (Java Persistence API):** A szabvány, ami összeköti a Java objektumokat az adatbázissal.
2.  **Entity (`@Entity`):** Java osztály = Adatbázis tábla.
3.  **EntityManager:** A munkás, aki végrehajtja a kéréseket (mentés, keresés).
4.  **JPQL:** SQL-szerű nyelv, de táblák helyett osztályokon operál.
5.  **Spring Data Repository:** Egy réteg a JPA fölött. Rengeteg kódot megspórol (pl. a `findBy...` metódusokkal), nem kell mindig `EntityManager`-rel bűvészkedni.
6.  **Transactional (`@Transactional`):** Biztosítja, hogy ha egy műveletsor közben hiba történik, minden visszaálljon az eredeti állapotra (rollback).