using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// SimpleBarricade - Prosta barykada blokująca tor
/// 
/// DLA STUDENTÓW:
/// ==============
/// To najprostsza przeszkoda w grze - statyczna barykada na torze.
/// 
/// Cechy:
/// ------
/// - Może być 1 lub 2 barykady obok siebie
/// - Losowe położenie na torach (lewy/środkowy/prawy)
/// - Specjalna obsługa pierwszej barykady w tutorialu
/// 
/// Dlaczego dziedziczy po Obstacle?
/// --------------------------------
/// Obstacle.cs ma wspólną logikę dla wszystkich przeszkód:
/// - Dźwięk uderzenia
/// - Animacja uderzenia
/// - Interface (Spawn, Impacted)
/// 
/// SimpleBarricade tylko implementuje JAK się spawni.
/// Reszta (uderzenie, dźwięk) jest w klasie bazowej.
/// 
/// Stałe (constants):
/// -----------------
/// k_ to prefix oznaczający stałą (convention Unity/Google)
/// 
/// k_MinObstacleCount = 1 - minimalna liczba barykad
/// k_MaxObstacleCount = 2 - maksymalna liczba barykad
/// k_LeftMostLaneIndex = -1 - lewy tor
/// k_RightMostLaneIndex = 1 - prawy tor
/// 
/// Indeksy torów:
/// - -1 = lewy tor
/// - 0 = środkowy tor
/// - 1 = prawy tor
/// 
/// Tutorial - pierwsza przeszkoda:
/// -------------------------------
/// Pierwsza barykada w tutorialu MUSI być:
/// - Tylko JEDNA (count = 1)
/// - Na torze ŚRODKOWYM (lane = 0)
/// 
/// Dlaczego?
/// Gracz uczy się sterowania i musi móc bezpiecznie uciec w DOWOLNĄ stronę.
/// Gdyby były dwie barykady lub była z boku, gracz mógłby nie wiedzieć gdzie uciec.
/// </summary>
public class SimpleBarricade : Obstacle
{
    // Minimalna liczba barykad do spawnu (1)
    protected const int k_MinObstacleCount = 1;
    // Maksymalna liczba barykad do spawnu (2)
    protected const int k_MaxObstacleCount = 2;
    // Indeks lewego toru (-1)
    protected const int k_LeftMostLaneIndex = -1;
    // Indeks prawego toru (1)
    protected const int k_RightMostLaneIndex = 1;
    
    /// <summary>
    /// Spawn - Spawnowanie barykady/barykad na torze
    /// 
    /// IEnumerator = Coroutine:
    /// -----------------------
    /// Funkcja może rozłożyć spawning na kilka klatek.
    /// Używamy tego bo Addressables ładuje zasoby ASYNCHRONICZNIE.
    /// 
    /// Parametry:
    /// ----------
    /// segment - segment toru na którym spawnimy
    /// t - pozycja na segmencie (0.0 - 1.0)
    /// 
    /// Algorytm:
    /// --------
    /// 1. Sprawdź czy to pierwsza przeszkoda tutorialu
    /// 2. Określ ile barykad (1-2) i na których torach
    /// 3. Pobierz pozycję na torze
    /// 4. Dla każdej barykady:
    ///    a) Załaduj prefab przez Addressables (asynchronicznie)
    ///    b) Ustaw pozycję (dodaj offset dla toru)
    ///    c) Przypisz jako dziecko segmentu toru
    /// </summary>
    public override IEnumerator Spawn(TrackSegment segment, float t)
    {
        // Sprawdź czy to pierwsza przeszkoda w tutorialu
        // Musi być spełnione:
        // - TrackManager jest w trybie tutorial
        // - To jest pierwsza przeszkoda (firstObstacle = true)
        // - To jest aktualny segment (gracz właśnie na nim stoi)
        bool isTutorialFirst = TrackManager.instance.isTutorial && TrackManager.instance.firstObstacle && segment == segment.manager.currentSegment;

        // Jeśli to pierwsza przeszkoda tutorialu, oznacz że już ją stworzyliśmy
        if (isTutorialFirst)
            TrackManager.instance.firstObstacle = false;
        
        // Określ liczbę barykad:
        // - Tutorial: ZAWSZE 1 barykada
        // - Normalnie: losowo 1 lub 2
        // Random.Range(min, max+1) bo drugi parametr jest exclusive (nie wliczony)
        int count = isTutorialFirst ? 1 : Random.Range(k_MinObstacleCount, k_MaxObstacleCount + 1);
        
        // Określ startowy tor:
        // - Tutorial: ZAWSZE środek (0)
        // - Normalnie: losowo lewy (-1), środek (0) lub prawy (1)
        int startLane = isTutorialFirst ? 0 : Random.Range(k_LeftMostLaneIndex, k_RightMostLaneIndex + 1);

        // Pobierz pozycję i rotację na torze w punkcie 't'
        Vector3 position;
        Quaternion rotation;
        segment.GetPointAt(t, out position, out rotation);

        // Spawnuj każdą barykadę
        for(int i = 0; i < count; ++i)
        {
            // Oblicz na którym torze będzie ta barykada
            int lane = startLane + i;
            // Jeśli wyszliśmy poza prawy tor, zawiń do lewego
            // Przykład: startLane=1, i=1 => lane=2 => zawiń do -1
            lane = lane > k_RightMostLaneIndex ? k_LeftMostLaneIndex : lane;

            // Addressables.InstantiateAsync - ASYNCHRONICZNE ładowanie prefaba
            // gameObject.name - nazwa prefaba do załadowania
            // position, rotation - gdzie i jak obrócić
            // 
            // Dlaczego async?
            // - Prefab może być na dysku, w internecie, w Asset Bundle
            // - Ładowanie może zająć czas
            // - Nie chcemy zamrozić gry podczas ładowania
            AsyncOperationHandle op = Addressables.InstantiateAsync(gameObject.name, position, rotation);
            
            // yield return - poczekaj aż ładowanie się skończy
            // Gra będzie działać normalnie podczas ładowania
            yield return op;
            
            // Sprawdź czy ładowanie się powiodło
            if (op.Result == null || !(op.Result is GameObject))
            {
                Debug.LogWarning(string.Format("Unable to load obstacle {0}.", gameObject.name));
                yield break; // Przerwij coroutine
            }
            
            // Rzutuj Result na GameObject
            GameObject obj = op.Result as GameObject;

            if (obj == null)
                Debug.Log(gameObject.name);
            else
            {
                // Przesuń barykadę w bok w zależności od toru
                // transform.right = wektor w prawo dla tego obiektu
                // lane * laneOffset = jak daleko w prawo
                //   lane = -1 => przesunięcie w lewo
                //   lane = 0 => brak przesunięcia
                //   lane = 1 => przesunięcie w prawo
                obj.transform.position += obj.transform.right * lane * segment.manager.laneOffset;

                // Ustaw segment jako rodzica (parent) barykady
                // true = zachowaj pozycję światową (world space)
                obj.transform.SetParent(segment.objectRoot, true);

                // TODO : remove that hack related to #issue7
                // To jest workaround dla buga w starszej wersji Unity/Addressables
                // Hack: przesuwamy obiekt w tył i z powrotem żeby "odświeżyć" pozycję
                Vector3 oldPos = obj.transform.position;
                obj.transform.position += Vector3.back; // Przesuń w tył
                obj.transform.position = oldPos; // Wróć do oryginału
            }
        }
    }
}
