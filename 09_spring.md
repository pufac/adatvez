Természetesen, következzen az utolsó diasor teljes, részletes és kommentárokkal ellátott magyarázata. Ez a prezentáció a Java világának két meghatározó vállalati keretrendszerét, az **EJB**-t és a **Spring**-et mutatja be.

---

### I. Rész: Java Enterprise Edition (Java EE) és EJB (1-19. dia)

**1. dia: Címlap**
Az előadás témája az **EJB (Enterprise JavaBeans)** és a **Spring**, a Java ökoszisztéma két kulcsfontosságú technológiája a szerveroldali, vállalati alkalmazások fejlesztéséhez.

**2-3. dia: Java Enterprise Edition (Java EE)**
*   **Mi az?** A Java EE (ma már **Jakarta EE**) nem egy konkrét szoftver, hanem egy **szabványgyűjtemény és architektúra** nagyvállalati (enterprise) alkalmazások fejlesztéséhez.
*   **Alkalmazásszerver:** Az a szoftver, ami a Java EE szabványokat megvalósítja. Az alkalmazásunkat ezen a szerveren futtatjuk.
*   **Architektúra:** Tipikusan háromrétegű. A kliens (böngésző) a webszerveren keresztül kommunikál az alkalmazásszerverrel, ami pedig JDBC segítségével éri el az adatbázist.

**4. dia: Java EE szolgáltatások**
Mit nyújt egy alkalmazásszerver a fejlesztőnek? Rengeteg "infrastrukturális" szolgáltatást, amivel nem kell a fejlesztőnek bajlódnia:
*   **Többszálúság, tranzakciókezelés, biztonság:** A szerver kezeli ezeket a bonyolult, alacsony szintű feladatokat.
*   **Perzisztencia:** Segítséget nyújt az objektumok adatbázisba mentéséhez (ORM).
*   **Távoli metódushívás (Remote Method Invocation - RMI):** Lehetővé teszi, hogy az egyik gépen futó program meghívja egy másik gépen futó program metódusát.
*   **Skálázhatóság, terhelés-kiegyenlítés:** Képes több szerver között elosztani a terhelést.

**5. dia: Java EE API-k**
Ezek a szolgáltatások szabványosított programozási felületeken (API-kon) keresztül érhetők el. Néhány fontosabb:
*   **JPA (Java Persistence API):** Az ORM szabvány (mint .NET-ben az Entity Framework).
*   **EJB (Enterprise JavaBeans):** Az üzleti logika komponensmodellje.
*   **JTA (Java Transaction API):** A tranzakciókezelés szabványa.
*   **Servlet, JSP, JSF:** Webes technológiák a felhasználói felülethez.
*   **JAX-WS, JAX-RS:** Webszolgáltatások (SOAP és REST) készítésének szabványai.
A hordozhatóság kulcsfontosságú: egy szabványos Java EE alkalmazásnak (elvileg) bármelyik Java EE-kompatibilis alkalmazásszerveren futnia kell.

**6. dia: Elterjedt Java EE alkalmazásszerverek**
A dia felsorol néhány ismert megvalósítást:
*   **Teljes szerverek:** Glassfish, WildFly (JBoss), IBM WebSphere, Oracle WebLogic.
*   **Csak Webkonténerek:** Az Apache Tomcat és a Jetty csak a webes szabványokat (pl. Servlet, JSP) valósítják meg, de az EJB-t nem.

**7-9. dia: Enterprise JavaBeans (EJB)**
*   **Fogalma:** Szabványos, szerveroldali komponensek az üzleti logika megvalósítására.
*   **EJB-konténer:** Az EJB-k egy konténerben futnak (ami az alkalmazásszerver része). A konténer **elfedi** a fejlesztő elől a bonyolult részleteket (hálózat, többszálúság, tranzakciók).
*   **EJB típusok:**
    1.  **Session Bean:** Az üzleti logikát tartalmazza. Ez a legfontosabb típus.
    2.  **Entity Bean:** Elavult ORM megoldás, a JPA teljesen leváltotta.
    3.  **Message-Driven Bean (MDB):** Aszinkron üzenetsorokból (pl. JMS) érkező üzenetek feldolgozására szolgál.

**10-15. dia: Explicit vs. Implicit Middleware**
Ez a rész az EJB (és a Spring) működésének kulcsát magyarázza el. A "middleware" itt a konténer által nyújtott szolgáltatások (tranzakciókezelés, biztonság) összessége.
*   **Explicit Middleware (rossz):** A fejlesztőnek kell a saját kódjában, manuálisan meghívnia a middleware szolgáltatásait (pl. `transaction.begin()`, `transaction.commit()`). Ez felduzzasztja és bonyolulttá teszi a kódot.
*   **Implicit Middleware (jó, EJB/Spring modell):** A fejlesztő csak a tiszta üzleti logikát írja meg. Azt, hogy milyen szolgáltatásokra van szüksége, egy külső **leíróban (XML) vagy annotációkkal** (`@Transactional`) adja meg.
    *   Amikor a kliens meghív egy metódust, a hívás valójában nem közvetlenül a mi objektumunkra érkezik, hanem egy, a **konténer által dinamikusan generált "wrapper" vagy "proxy" objektumra** (a dián "Kérésmegszakító").
    *   Ez a proxy objektum a metódushívás **előtt** elindítja a szükséges szolgáltatásokat (pl. tranzakciót nyit), majd **meghívja a mi eredeti metódusunkat**, végül a hívás **után** lezárja a szolgáltatásokat (pl. commit-álja a tranzakciót).
    *   Ez a technika (az **Aspect-Oriented Programming - AOP** egyik formája) teszi lehetővé, hogy az üzleti logika tiszta és független maradjon az infrastrukturális kódtól.

**16-18. dia: EJB példányok és állapotkezelés**
*   **Referencia szerzése:** EJB-t sosem a `new` kulcsszóval hozunk létre. A konténertől kérjük el egy névszolgáltatás (JNDI) vagy modern EJB-ben **dependency injection** (`@EJB` annotáció) segítségével.
*   **Szálkezelés:** A konténer garantálja, hogy egy EJB példányt egyszerre csak egy szál használ, így a fejlesztőnek nem kell a szinkronizációval törődnie. A konténer egy **példány-készletet (instance pool)** tart fenn, hogy több klienst párhuzamosan ki tudjon szolgálni.
*   **Állapotkezelés (Session Bean típusai):**
    1.  **`@Stateless` (Állapotmentes):** A kliens minden hívása a pool-ból egy tetszőleges példányhoz kerülhet. Emiatt a bean nem tárolhat kliens-specifikus állapotot a hívások között. Nagyon hatékony és jól skálázható.
    2.  **`@Stateful` (Állapottartó):** A konténer biztosítja, hogy egy adott kliens minden hívása ugyanahhoz a bean példányhoz fusson be. Így a bean tárolhat állapotot (pl. egy több lépéses varázsló adatai).
    3.  **`@Singleton`:** Az egész alkalmazásban csak egyetlen példány létezik belőle, amit minden kliens közösen használ.

**19-25. dia: JPA és Tranzakciók EJB környezetben**
*   **Menedzselt perzisztenciakontextus:** EJB-ben a JPA `EntityManager`-t (ami a `DbContext` Java megfelelője) a konténer menedzseli. Elég egy `@PersistenceContext` annotációval elkérni.
*   **Tranzakciók élettartama:** Alapértelmezetten az `EntityManager` a tranzakció élettartamához van kötve. Amikor egy EJB metódus elindul, a konténer indít egy tranzakciót és létrehoz egy új `EntityManager`-t. Amikor a metódus véget ér, a tranzakció commit-álódik és az `EntityManager` bezáródik.
*   **Tranzakciókezelés módjai:**
    1.  **Bean-Managed (BMT - Programozott):** A fejlesztő manuálisan kezeli a tranzakciókat a kódban. Ritkán használatos.
    2.  **Container-Managed (CMT - Deklaratív):** **Ez az alapértelmezett és javasolt módszer.** A konténer kezeli a tranzakciókat az EJB metódusokra helyezett annotációk (`@TransactionAttribute`) alapján.

*   **Tranzakciós attribútumok (`@TransactionAttribute`):** Ezek szabályozzák, hogyan viselkedjen egy metódus egy már meglévő tranzakció kontextusában. A legfontosabbak:
    *   **`REQUIRED` (alapértelmezett):** Ha van már tranzakció, csatlakozik hozzá. Ha nincs, újat indít.
    *   **`REQUIRES_NEW`:** Mindig új, független tranzakciót indít. A hívó tranzakcióját felfüggeszti.
    *   **`SUPPORTS`:** Ha van tranzakció, csatlakozik, ha nincs, tranzakció nélkül fut.
    *   **`NOT_SUPPORTED`:** Mindig tranzakció nélkül fut, a hívó tranzakcióját felfüggeszti.

---

### II. Rész: Spring Keretrendszer (31-59. dia)

**31-32. dia: A Spring alapelvei**
A Spring eredetileg a korai (1.x, 2.x) EJB bonyolultságára adott, könnyebb és rugalmasabb alternatívaként indult. Azóta a Java világ legnépszerűbb keretrendszerévé nőtte ki magát. Fő alapelvei:
*   **Egyszerűség:** POJO-kra (Plain Old Java Object - "sima" Java osztályok) épül, nincs szükség speciális ősosztályokra.
*   **Modularitás:** Rétegzett felépítésű, csak azokat a modulokat kell használnod, amikre szükséged van.
*   **Tesztelhetőség:** A komponensek legyenek izoláltan, könnyen tesztelhetők.

**33-34. dia: Mit nyújt a Spring?**
Hasonló szolgáltatásokat nyújt, mint egy Java EE alkalmazásszerver, de egy "könnyűsúlyú" konténer formájában, ami akár egy egyszerű Tomcat webkonténeren vagy egy sima Java alkalmazásban is futhat.
1.  **Core Container (DI/IoC):** A Spring magja, ami az objektumok létrehozásáért, konfigurálásáért és "összedrótozásáért" felelős.
2.  **Egységes tranzakciókezelés:** Absztrakt réteget biztosít a különböző tranzakciós technológiák fölé.
3.  **Adatelérés támogatás:** Segédosztályokat nyújt a JDBC, JPA, Hibernate stb. egyszerűbb használatához.
4.  **AOP támogatás:** Lehetővé teszi a deklaratív szolgáltatásokat (mint a tranzakciókezelés).
5.  **Webkeretrendszer (Spring MVC):** Egy teljes értékű MVC keretrendszer webalkalmazások készítéséhez.

**35-42. dia: Függőséginjektálás (Dependency Injection - DI)**
Ez a Spring legfontosabb alapelve, és az **Inversion of Control (IoC - Vezérlés megfordítása)** elv egy megvalósítása.
*   **A probléma:** Hagyományosan egy objektum maga felelős a saját függőségeinek (az objektumoknak, amikre szüksége van a működéséhez) a létrehozásáért.

    ```java
    public class CommandService {
        private SettingsService settingsService;

        public CommandService() {
            // A CommandService maga hozza létre a függőségét.
            // Ez szoros csatolást (tight coupling) eredményez.
            this.settingsService = new SettingsService();
        }
        // ...
    }
    ```
*   **A DI megoldás:** A vezérlés megfordul. Az objektum nem hozza létre a függőségeit, hanem **kívülről kapja meg őket** (injektálják neki), tipikusan a konstruktorán vagy egy setter metóduson keresztül.

    ```java
    public class CommandService {
        private SettingsService settingsService;

        // A függőséget a konstruktoron keresztül kapja meg.
        public CommandService(SettingsService settings) {
            this.settingsService = settings;
        }
        // ...
    }
    ```
*   **A Spring IoC Konténer:** Egy külső "gyár", ami beolvassa a konfigurációt, létrehozza az objektumokat (amiket Springben **bean**-eknek hívunk), és automatikusan "beadja" (injektálja) nekik a függőségeiket, majd összerakja a teljes objektumgráfot.
*   **Konfiguráció evolúciója:**
    1.  **XML fájlok:** Kezdetben a beaneket és a függőségeiket XML-ben kellett leírni.
    2.  **Annotációk (`@Autowired`, `@Component`):** Az osztályok annotálásával a Spring automatikusan felderíti a beaneket és összekapcsolja őket.
    3.  **JavaConfig (`@Configuration`, `@Bean`):** Tiszta Java kódban, típus-biztosan lehet definiálni a konfigurációt.
    4.  **Spring Boot:** A modern megközelítés, ami "convention over configuration" alapon, minimális konfigurációval automatikusan beállít szinte mindent.

**43-52. dia: Adatelérés és JPA Támogatás Springben**
*   **DAO támogatás:** A Spring Data Access Object (DAO) modulja leegyszerűsíti az adatelérést.
*   **`JDBCTemplate`:** "Wrapper" a standard JDBC köré, ami rengeteg ismétlődő (boilerplate) kódtól kíméli meg a fejlesztőt (kapcsolat nyitása/zárása, `try-catch-finally` blokkok).
*   **JPA integráció:** A Spring tökéletesen integrálódik a JPA-val.
    *   Az `EntityManagerFactory`-t és az `EntityManager`-t Spring bean-ként lehet konfigurálni.
    *   Az `@PersistenceContext` annotációval a Spring konténer be tudja injektálni az `EntityManager`-t a repository osztályainkba.
*   **Spring Data JPA:** Ez egy még magasabb szintű absztrakció. Elég létrehozni egy interfészt, ami kiterjeszti a `JpaRepository`-t, és a Spring Data futásidőben, **a metódusnevek alapján automatikusan legenerálja a lekérdezések implementációját!**

    ```java
    // A fejlesztőnek csak ezt az interfészt kell megírnia.
    public interface PersonRepository extends JpaRepository<Person, Long> {

        // A Spring Data a név alapján tudni fogja, hogy egy
        // "SELECT p FROM Person p WHERE p.lastname = ?" lekérdezést kell generálnia.
        List<Person> findByLastname(String lastname);
    }
    ```
    A Spring Data a Repository minta egy nagyon erőteljes, "varázslatos" megvalósítása.

**53-59. dia: Tranzakciókezelés Springben**
*   **Egységes API:** A Spring egy `PlatformTransactionManager` interfészt biztosít, ami elrejti a konkrét technológia (JDBC, JTA, Hibernate stb.) részleteit.
*   **Deklaratív tranzakciókezelés:** Az EJB-hez nagyon hasonlóan működik, a `@Transactional` annotáció segítségével.

    ```java
    @Service
    public class LogService {
        @PersistenceContext
        private EntityManager em;

        // A Spring konténer biztosítja, hogy ez a metódus egy tranzakcióban fusson le.
        // Hívás előtt tranzakciót nyit, utána commit-ál (vagy hiba esetén rollback-el).
        @Transactional
        public void create(LogItem logItem) {
            em.persist(logItem);
        }
    }
    ```
*   **@Transactional paraméterei:** Nagyon hasonlítanak az EJB-s megfelelőjükhöz (`propagation`, `isolation`, `rollbackFor` stb.), lehetővé téve a tranzakciók viselkedésének finomhangolását. A legfontosabb különbség, hogy Springben **alapértelmezetten nincs tranzakció**, azt expliciten jelölni kell az annotációval, míg EJB-ben a `REQUIRED` az alapértelmezett.

# nem értettem semmit igy reszletesebben ↓

Teljesen érthető, hogy ez az anyag tűnik a legbonyolultabbnak a vizsgaanyagok közül. A korábbi témák (SQL, adatbázis-tervezés) konkrétabbak voltak. Az EJB és a Spring világa viszont nagyon absztrakt, tele van tervezési mintákkal és "mágiával", ami a háttérben történik.

Ne aggódj, most végigmegyünk rajta lépésről lépésre, minden egyes diát alaposan kibontva. A cél, hogy ne csak a "mit", hanem a "miért" is a helyére kerüljön. Képzeld el, hogy egy komplex gépet építünk: eddig az alkatrészeket (adatbázis, táblák) néztük, most pedig a gépet összerakó keretrendszereket és a mögöttük lévő filozófiát vizsgáljuk meg.

---

### I. Rész: Java Enterprise Edition (Java EE) és az EJB Világa

Ez a fejezet a Java "hivatalos", szabványokon alapuló vállalati megoldásáról szól.

**2-3. dia: Java Enterprise Edition (Java EE)**

*   **Miről van szó?** A Java EE (ma már Jakarta EE néven fut) nem egy program, amit letöltesz, hanem egy **szabványgyűjtemény**, egy "szakácskönyv" nagy, megbízható, szerveroldali alkalmazások készítéséhez. Azt írja le, hogy egy vállalati rendszernek milyen komponensekből kell állnia és azoknak hogyan kell viselkedniük.
*   **Alkalmazásszerver:** Az a konkrét szoftver (pl. WildFly, Glassfish), ami ezt a "szakácskönyvet" megvalósítja. Olyan, mint egy futtatókörnyezet a programjainknak, ami rengeteg terhet levesz a vállunkról.
*   **Háromrétegű architektúra:** Az ábra egy klasszikus háromrétegű modellt mutat:
    1.  **Kliens (Megjelenítési réteg):** A felhasználó böngészője.
    2.  **Web szerver + Alkalmazás szerver (Üzleti logikai réteg):** Itt fut a mi Java kódunk. A webszerver a statikus tartalmakat (HTML) szolgálja ki, az alkalmazásszerver pedig a dinamikus logikát futtatja.
    3.  **Adatbázis (Adat réteg):** Ahol az adatok laknak.

**4. dia: Java EE szolgáltatások**

Ez a legfontosabb "miért" az egészben. Miért használjunk egyáltalán alkalmazásszervert? Mert rengeteg bonyolult, minden projektben előkerülő problémát megold helyettünk. Ezeket hívjuk **infrastrukturális szolgáltatásoknak**:

*   **Többszálúság:** Nem neked kell a szálakkal és a zárolással bajlódnod, hogy egyszerre több felhasználót is ki tudj szolgálni. A szerver ezt kezeli.
*   **Tranzakciókezelés:** Garantálja, hogy az adatbázis-műveletek vagy teljesen lefutnak, vagy sehogy (ACID).
*   **Biztonság:** Felhasználókezelés, jogosultságok ellenőrzése.
*   **Perzisztencia:** Segítség az objektumaink adatbázisba mentéséhez.
*   **Skálázhatóság, Terhelés-kiegyenlítés:** Képes több gépen elosztani a munkát, ha megnő a terhelés.

A fejlesztőnek így csak a **valódi üzleti problémával** kell foglalkoznia.

**5. dia: Java EE API-k**

A fenti szolgáltatásokat szabványosított programozási felületeken (API) keresztül érjük el.
*   **JPA (Java Persistence API):** Az adatok mentésére és lekérdezésére. Ez az Entity Framework Java megfelelője.
*   **EJB (Enterprise JavaBeans):** Az üzleti logika megírására szolgáló komponensmodell.
*   **JTA (Java Transaction API):** A tranzakciók kezelésére.
*   **A lényeg:** Ha ezeket a szabványos API-kat használod, az alkalmazásod **hordozható** lesz a különböző alkalmazásszerverek között (pl. JBoss-ról átteheted WebSphere-re).

**6-8. dia: Enterprise JavaBeans (EJB) fogalma**

*   **Definíció:** Az EJB egy szabványos komponensmodell az üzleti logika megvalósítására. Képzeld el őket úgy, mint speciális Java osztályokat, amik egy védett környezetben, az **EJB-konténerben** futnak.
*   **EJB-konténer:** Ez az alkalmazásszervernek az a része, ami közvetlenül az EJB-jeinkkel foglalkozik. Olyan, mint egy személyi menedzser vagy egy komornyik az objektumaink számára: **elfedi** a külvilág bonyolultságát. A mi EJB osztályunk csak a tiszta logikát tartalmazza, a konténer pedig gondoskodik a hálózati kommunikációról, a tranzakciókról, a biztonságról stb.

**9. dia: Az EJB-k típusai**

1.  **Session Bean:** Ez a legfontosabb. Az üzleti logikát, a "mit csináljon a program" részt ide írjuk. (Pl. egy `OrderService` bean, aminek van egy `placeOrder` metódusa). Erre fókuszálunk.
2.  **Entity Bean:** Egy nagyon régi, elavult technológia az adatbázis-objektumok leképezésére. **Ma már senki nem használja**, a helyét teljesen átvette a **JPA**.
3.  **Message-Driven Bean (MDB):** Aszinkron üzenetek feldolgozására való. Akkor használjuk, ha a rendszerünknek egy üzenetsorból (pl. RabbitMQ, ActiveMQ) kell fogadnia és feldolgoznia az üzeneteket.

**10-13. dia: Explicit vs. Implicit Middleware – A konténer "mágiája"**

Ez a legfontosabb koncepció az egészben! A "middleware" a konténer által nyújtott összes szolgáltatás (tranzakció, biztonság stb.).
*   **Explicit Middleware (A ROSSZ út):** A 10. ábra ezt mutatja. A fejlesztőnek kellene a saját `Elosztott objektum` kódjába beleírnia a tranzakció indítását, a biztonsági ellenőrzést stb. Ez ahhoz vezet, hogy a kód tele lesz technikai "zajjal", nehezen olvasható és karbantartható lesz (11. dia).

*   **Implicit Middleware (A JÓ út, az EJB és Spring modellje):** A 12. ábra mutatja. A mi `Elosztott objektum` kódunk csak a tiszta üzleti logikát tartalmazza.
    *   **A trükk:** Amikor egy kliens meghív egy metódust, a hívás nem közvetlenül a mi objektumunkra érkezik. A konténer egy láthatatlan **proxy** ("Kérésmegszakító") réteget tesz a mi objektumunk köré.
    *   A hívás a **proxy-ra** érkezik.
    *   A proxy megnézi a konfigurációt (XML vagy annotációk), és a metódushívás **előtt** elvégzi a szükséges feladatokat (pl. elindítja a tranzakciót).
    *   Ezután meghívja a **mi eredeti metódusunkat**.
    *   Miután a mi metódusunk lefutott, a vezérlés visszakerül a proxy-hoz, ami elvégzi a záró feladatokat (pl. `commit` vagy `rollback`).
    *   Ez a technika **szétválasztja az üzleti logikát az infrastrukturális kódtól**, ami tisztább, karbantarthatóbb kódot eredményez.

**14-15. dia: Session Bean felépítése és egy EJB hívás folyamata**

*   **Felépítés:** Egy EJB tipikusan egy interfészből (amit a kliens lát) és egy implementációs osztályból (ahol a logika van) áll.
*   **A hívás folyamata (15. dia):** Ez az ábra pontosan az implicit middleware működését mutatja be lépésről lépésre.
    1.  Kliens hívja a metódust (a `Business Interface`-en keresztül).
    2.  A hívás a konténer által generált `Wrapper Class`-ra (proxy) érkezik.
    3.  A proxy a hívás **előtt** igénybe veszi a konténer szolgáltatásait (pl. tranzakció indítása).
    4.  A proxy meghívja a mi `Enterprise Bean Class`-unkban lévő üzleti logikát.
    5.  A mi kódunk lefutása **után** a proxy ismét a konténerhez fordul (pl. a tranzakció lezárásához).
    6.  A proxy visszaadja az eredményt a kliensnek.

**16-18. dia: Példányosítás és Állapotkezelés**

*   **Miért nem használhatunk `new`-t?** Mert ha mi magunk példányosítanánk az osztályunkat, a konténer nem tudná köré tenni a proxy-t, így elveszne az összes szolgáltatás. Ehelyett a konténertől kell "elkérni" egy példányt, ami valójában a proxy-ra mutató referencia lesz. Ezt ma már **függőséginjektálással** (`@EJB` annotáció) oldjuk meg.
*   **Szálkezelés:** A konténer egy **instance pool**-t (példánykészletet) tart fenn. Amikor bejön egy kérés, kivesz egy szabad példányt a pool-ból, odaadja a kérésnek, majd a kérés végén visszateszi. Ez garantálja, hogy egy példányt egyszerre csak egy szál használ, így a fejlesztőnek nem kell a szinkronizációval foglalkoznia.
*   **Állapotkezelés (18. dia):** A nagy kérdés az, hogy a kliens a következő hívásakor ugyanazt a példányt kapja-e meg a pool-ból?
    *   **`@Stateless` (Állapotmentes):** Nem garantált. Bármelyik szabad példányt megkaphatja. Ezért a bean nem tárolhat hívások között megmaradó állapotot. **Ez a leggyakoribb és leghatékonyabb típus.**
    *   **`@Stateful` (Állapottartó):** Garantált. A konténer "hozzáköti" a példányt a klienshez. Ez állapotot tud tárolni (pl. egy webshop kosár tartalma), de több erőforrást igényel.
    *   **`@Singleton`:** Az egész alkalmazásban egyetlen példány van, minden kliens ugyanazt használja.

**19-22. dia: JPA használata EJB környezetben**

*   **`@PersistenceContext`:** Ezzel az annotációval tudunk egy `EntityManager` példányt (a JPA központi objektuma, hasonlít az EF `DbContext`-jére) **injektáltatni** az EJB-nkbe. A konténer gondoskodik a létrehozásáról és életciklusának kezeléséről.

    ```java
    @Stateless
    public class PersonService {
        // A konténer automatikusan értéket ad ennek a mezőnek.
        @PersistenceContext
        private EntityManager em;

        public void createEmployee() {
            // Az injektált 'em' használata.
            em.persist(new Employee(12345, "Gabor"));
        }
    }
    ```
*   **Élettartam:** Alapértelmezetten (`TRANSACTION` type) az `EntityManager` a tranzakcióval együtt él. A metódus elején létrejön, a végén bezáródik.

**23-30. dia: Tranzakciókezelés EJB-ben**

*   **Szereplők:** Tranzakció menedzser (a konténer része), erőforrás menedzser (pl. JDBC driver).
*   **Két mód (26. dia):**
    1.  **`@TransactionManagement(BEAN)` (Programozott):** A fejlesztő írja a kódban, hogy `transaction.begin()`, `commit()`, `rollback()`. Nagyon ritkán használatos.
    2.  **`@TransactionManagement(CONTAINER)` (Deklaratív):** **Ez az alapértelmezett és a javasolt út.** A konténer kezeli a tranzakciókat a metódusokra helyezett annotációk alapján.
*   **Tranzakciós attribútumok (29. dia):** Ezek az annotációk (`@TransactionAttribute`) mondják meg a konténernek, hogyan kezelje a tranzakciót az adott metódus hívásakor.
    *   **`REQUIRED`:** (Alapértelmezett) Ha a hívónak már van tranzakciója, csatlakozik hozzá. Ha nincs, újat indít. Ez a leggyakoribb.
    *   **`REQUIRES_NEW`:** Mindig új, független tranzakciót indít. (Pl. egy naplózó metódus, aminek akkor is sikeresnek kell lennie, ha a fő művelet hibára fut).
    *   A többi ritkábban használt, specifikus esetekre való.

---

### II. Rész: Spring Keretrendszer

A Spring egy alternatív, könnyebb és rugalmasabb keretrendszer, ami mára a Java világ de facto szabványává vált.

**31-33. dia: A Spring alapelvei és céljai**

*   **Cél:** A korai EJB bonyolultságának egyszerűsítése. A Spring megmutatta, hogy a Java EE szolgáltatásai egy "könnyűsúlyú" konténerrel, sima Java objektumokkal (POJO-kkal) is megvalósíthatók.
*   **Függőséginjektálás (DI) / Inversion of Control (IoC):** Ez a Spring lelke. Ahelyett, hogy egy objektum maga hozná létre a függőségeit (`new ...`), a konténer adja oda neki őket kívülről. Ez lazább csatolást és jobb tesztelhetőséget eredményez.
*   **AOP (Aspect-Oriented Programming):** Ugyanaz az implicit middleware koncepció, mint az EJB-nél. A Spring is proxy-kat használ, hogy deklaratív szolgáltatásokat (pl. tranzakciókezelés) nyújtson.

**35-42. dia: Függőséginjektálás (DI) a gyakorlatban**

*   **A probléma (POJO-k):** Adott egy `CommandService`, aminek szüksége van egy `SettingsService`-re.
*   **A megoldás (DI):** A `CommandService` nem példányosítja a `SettingsService`-t, hanem egy setter metóduson keresztül kapja meg. Fontos, hogy a kód **semmilyen Spring API-t nem használ**, teljesen független a keretrendszertől!
*   **A "drótozás" (Wiring):** Hogyan mondjuk meg a Springnek, hogy a `CommandService`-nek a `SettingsService`-t kell odaadnia?
    1.  **XML Konfiguráció (régi):** Egy `beans.xml` fájlban leírjuk a komponenseket ("bean"-eket) és a kapcsolataikat.

        ```xml
        <!-- Létrehoz egy SettingsService példányt 'settingsService' néven -->
        <bean id="settingsService" class="...SettingsService"/>

        <!-- Létrehoz egy CommandService példányt 'commandService' néven -->
        <bean id="commandService" class="...CommandService">
            <!-- Meghívja a 'setSettingsService' metódust, és odaadja neki a 'settingsService' bean-t -->
            <property name="settingsService" ref="settingsService"/>
        </bean>
        ```
    2.  **Annotáció-alapú konfiguráció (modern):** Az osztályokat megjelöljük `@Service` vagy `@Component` annotációval, a függőséget pedig `@Autowired`-del. A Spring automatikusan felderíti és összekapcsolja őket.

        ```java
        @Service
        public class CommandService {
            @Autowired // Spring, kérlek ide injektálj egy SettingsService példányt!
            private SettingsService settingsService;
            // ...
        }

        @Service
        public class SettingsService { ... }
        ```
    3.  **JavaConfig (legmodernebb):** Egy tiszta Java osztályban, `@Configuration` és `@Bean` annotációkkal definiáljuk a beaneket.

**43-52. dia: Adatelérés támogatása Springben**

*   **`JDBCTemplate`:** Egy segédosztály, ami a "nyers" JDBC használatát egyszerűsíti le, automatizálva az erőforrások kezelését.
*   **JPA használata:** A Spring tökéletesen integrálódik a JPA-val. Az `@PersistenceContext` annotáció itt is ugyanúgy működik, mint EJB-ben, a Spring konténer injektálja az `EntityManager`-t.
*   **Spring Data JPA:** A Spring adatelérési támogatásának csúcsa. Lehetővé teszi, hogy az adatelérési réteg (repository) szinte teljes egészében **automatikus kódgenerálással** készüljön el.
    *   Csak egy interfészt kell írnunk, ami kiterjeszti a `JpaRepository<T, ID>`-t.
    *   Az alap CRUD műveleteket (`save`, `findById`, `findAll`, `delete`) "ingyen" megkapjuk.
    *   **Generált lekérdezések:** Ha az interfészbe felveszünk egy metódust egy speciális névkonvenció szerint (pl. `findByLastname`), a Spring Data a metódus nevéből kitalálja a szükséges SQL lekérdezést és legenerálja az implementációt.

    ```java
    // A fejlesztőnek CSAK ennyi kódot kell írnia!
    public interface PersonRepository extends JpaRepository<Person, Long> {
        // A Spring ebből tudni fogja, hogy egy WHERE lastname = ? lekérdezést kell generálnia.
        List<Person> findByLastname(String lastname);

        // Ebből egy WHERE lastname = ? OR firstname = ? lekérdezés lesz.
        List<Person> findByLastnameOrFirstname(String lastname, String firstname);
    }
    ```
    Ha ennél bonyolultabb kell, a `@Query` annotációval saját lekérdezést is megadhatunk.

**53-59. dia: Tranzakciókezelés Springben**

*   **Egységes API:** A Spring egy absztrakt `PlatformTransactionManager`-t használ, így a kódunk független a konkrét tranzakciós technológiától.
*   **Deklaratív tranzakciókezelés (`@Transactional`):** Pontosan úgy működik, mint az EJB-ben. A metódus fölé tett `@Transactional` annotáció jelzi a Spring konténernek, hogy a metódus hívását egy adatbázis-tranzakcióba kell csomagolnia.

    ```java
    @Service
    public class LogService {
        @PersistenceContext
        private EntityManager em;

        // Ez a metódus tranzakcionálisan fog lefutni.
        @Transactional
        public void create(LogItem logItem) {
            em.persist(logItem);
        }
    }
    ```
*   **Fontos különbség az EJB-hez képest:** Springben a tranzakciós viselkedést **explicit ki kell tenni** a `@Transactional` annotációval. Az EJB-ben alapértelmezetten minden metódus tranzakcionális (`REQUIRED`).
*   Az annotáció paraméterei (`propagation`, `isolation` stb.) lehetővé teszik a tranzakciók viselkedésének finomhangolását, az EJB-ből ismert módon.