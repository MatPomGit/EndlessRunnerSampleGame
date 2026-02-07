using UnityEngine;
using System.Collections;

/// <summary>
/// CoroutineHandler - Handler do uruchamiania Coroutine z klas nie-MonoBehaviour
/// 
/// DLA STUDENTÓW - ZAAWANSOWANA KONCEPCJA:
/// ========================================
/// 
/// Problem do rozwiązania:
/// -----------------------
/// Coroutines w Unity mogą być uruchomione TYLKO z MonoBehaviour.
/// 
/// Co to są Coroutines?
/// -------------------
/// Coroutine to funkcja, która może "pauzować" wykonanie i kontynuować później.
/// Normalnie funkcja działa OD POCZĄTKU DO KOŃCA w jednej klatce.
/// Coroutine może rozłożyć swoją pracę na wiele klatek.
/// 
/// Przykład:
/// 
/// // Normalna funkcja - ZABLOKUJE GRĘ na 5 sekund!
/// void Wait5Seconds() {
///     Thread.Sleep(5000); // ŹLE! Gra zamarznie!
/// }
/// 
/// // Coroutine - NIE blokuje, czeka w tle
/// IEnumerator Wait5Seconds() {
///     yield return new WaitForSeconds(5); // Gra działa normalnie!
/// }
/// 
/// Kiedy używać Coroutines?
/// ------------------------
/// - Animacje/ruchy rozłożone w czasie
/// - Opóźnienia (wait X sekund, potem zrób Y)
/// - Stopniowe ładowanie zasobów
/// - Efekty specjalne (fade in/out)
/// 
/// Problem:
/// --------
/// StartCoroutine() istnieje TYLKO w klasach MonoBehaviour!
/// 
/// Jeśli masz zwykłą klasę C# (nie dziedziczy po MonoBehaviour):
/// 
/// public class MyClass {
///     void DoSomething() {
///         StartCoroutine(MyCoroutine()); // BŁĄD! Nie ma StartCoroutine!
///     }
/// }
/// 
/// Rozwiązanie - CoroutineHandler:
/// --------------------------------
/// Tworzymy JEDEN GameObject z MonoBehaviour, który będzie uruchamiać Coroutines
/// dla wszystkich innych klas.
/// 
/// To jest wzorzec "Singleton" + "Proxy":
/// - Singleton: Tylko JEDNA instancja w całej grze
/// - Proxy: Pośrednik między klasami non-MonoBehaviour a Unity
/// 
/// Użycie:
/// -------
/// CoroutineHandler.StartStaticCoroutine(MyCoroutine());
/// 
/// DontDestroyOnLoad:
/// ------------------
/// Normalnie gdy zmieniamy scenę, wszystkie GameObjecty są niszczone.
/// DontDestroyOnLoad() mówi Unity: "NIE niszcz tego obiektu przy zmianie sceny"
/// Dzięki temu CoroutineHandler żyje przez CAŁĄ grę.
/// </summary>
public class CoroutineHandler : MonoBehaviour
{
    // Singleton - jedna instancja dla całej gry
    static protected CoroutineHandler m_Instance;
    
    /// <summary>
    /// instance - Właściwość zwracająca singleton
    /// 
    /// Lazy Initialization (leniwa inicjalizacja):
    /// -------------------------------------------
    /// Obiekt NIE jest tworzony od razu, tylko gdy po raz pierwszy jest potrzebny.
    /// 
    /// Pierwsze wywołanie instance:
    /// 1. m_Instance == null? TAK
    /// 2. Tworzymy GameObject "CoroutineHandler"
    /// 3. DontDestroyOnLoad - przeżyje zmiany scen
    /// 4. AddComponent<CoroutineHandler>() - dodajemy ten skrypt
    /// 5. Zapisujemy referencję w m_Instance
    /// 
    /// Kolejne wywołania instance:
    /// 1. m_Instance == null? NIE
    /// 2. Zwracamy istniejącą instancję
    /// </summary>
    static public CoroutineHandler instance
    {
        get
        {
            // Jeśli instancja nie istnieje, stwórz ją
            if(m_Instance == null)
            {
                // Twórz nowy GameObject w scenie
                GameObject o = new GameObject("CoroutineHandler");
                
                // DontDestroyOnLoad - ten obiekt PRZEŻYJE zmianę sceny
                // Normalnie wszystkie obiekty są niszczone przy LoadScene
                DontDestroyOnLoad(o);
                
                // Dodaj ten komponent do GameObjecta
                // Teraz GameObject "CoroutineHandler" ma skrypt CoroutineHandler
                m_Instance = o.AddComponent<CoroutineHandler>();
            }

            return m_Instance;
        }
    }

    /// <summary>
    /// OnDisable - wywoływane gdy komponent jest wyłączany
    /// 
    /// Sprzątanie:
    /// -----------
    /// Gdy gra się zamyka lub komponent jest niszczony, musimy posprzątać.
    /// Niszczymy GameObject żeby nie było "memory leak" (wycieku pamięci).
    /// </summary>
    public void OnDisable()
    {
        if(m_Instance)
            Destroy(m_Instance.gameObject);
    }

    /// <summary>
    /// StartStaticCoroutine - Statyczna funkcja do uruchamiania Coroutines
    /// 
    /// To jest funkcja którą NAPRAWDĘ używasz z zewnątrz!
    /// 
    /// Przykład użycia:
    /// ---------------
    /// 
    /// // W jakiejkolwiek klasie (nawet non-MonoBehaviour):
    /// IEnumerator FadeOut() {
    ///     float alpha = 1f;
    ///     while(alpha > 0) {
    ///         alpha -= Time.deltaTime;
    ///         // ustaw przezroczystość
    ///         yield return null; // Czekaj jedną klatkę
    ///     }
    /// }
    /// 
    /// // Uruchom:
    /// CoroutineHandler.StartStaticCoroutine(FadeOut());
    /// 
    /// Co się dzieje:
    /// -------------
    /// 1. instance - pobiera (lub tworzy) singleton
    /// 2. instance.StartCoroutine() - uruchamia coroutine NA tym singletonie
    /// 3. Zwraca referencję do Coroutine (możesz ją później zatrzymać)
    /// 
    /// Parametr:
    /// --------
    /// IEnumerator - to typ zwracany przez funkcje z "yield return"
    /// </summary>
    static public Coroutine StartStaticCoroutine(IEnumerator coroutine)
    {
        // instance automatycznie stworzy singleton jeśli nie istnieje
        // StartCoroutine jest funkcją z MonoBehaviour - działa bo
        // CoroutineHandler dziedziczy po MonoBehaviour!
        return instance.StartCoroutine(coroutine);
    }
}
