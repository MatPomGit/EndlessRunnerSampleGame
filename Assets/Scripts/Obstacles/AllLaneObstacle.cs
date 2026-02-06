using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// AllLaneObstacle - Przeszkoda blokująca WSZYSTKIE tory naraz
/// 
/// DLA STUDENTÓW:
/// ==============
/// To przeszkoda której NIE MOŻNA ominąć biegając w bok!
/// Blokuje cały tor - wszystkie 3 pasy.
/// 
/// Jak ją ominąć?
/// -------------
/// Gracz MUSI:
/// - Skoczyć NAD przeszkodą, LUB
/// - Zślizgnąć się POD przeszkodą
/// 
/// Przykłady w grze:
/// ----------------
/// - Wysoki tunel (musisz się zślizgnąć pod nim)
/// - Niska przeszkoda na ziemi rozciągnięta na całą szerokość
/// 
/// Różnica od SimpleBarricade:
/// ---------------------------
/// SimpleBarricade:
/// - Blokuje 1-2 tory
/// - Gracz może zmienić tor żeby ją ominąć
/// - Nie wymaga skoku/ślizgu
/// 
/// AllLaneObstacle:
/// - Blokuje WSZYSTKIE tory
/// - Gracz MUSI skoczyć lub się zślizgnąć
/// - Bardziej trudna!
/// 
/// Kod:
/// ----
/// Zauważ że Spawn() jest DUŻO prostszy niż w SimpleBarricade.
/// Dlaczego?
/// - Nie ma losowania liczby przeszkód (zawsze 1)
/// - Nie ma losowania toru (blokuje wszystkie)
/// - Nie ma specjalnej obsługi tutorialu
/// 
/// Po prostu spawnimy JEDEN obiekt na pozycji 't' segmentu.
/// </summary>
public class AllLaneObstacle: Obstacle
{
    /// <summary>
    /// Spawn - Spawnowanie przeszkody blokującej wszystkie tory
    /// 
    /// Prostszy niż SimpleBarricade bo:
    /// - Zawsze JEDNA przeszkoda
    /// - Zawsze na ŚRODKU (blokuje wszystkie tory)
    /// - Brak specjalnej logiki tutorialu
    /// 
    /// Algorytm:
    /// --------
    /// 1. Pobierz pozycję na segmencie toru
    /// 2. Załaduj prefab asynchronicznie (Addressables)
    /// 3. Ustaw jako dziecko segmentu
    /// 
    /// To jest podstawowa wersja Spawn() - minimum potrzebne do działania.
    /// </summary>
	public override IEnumerator Spawn(TrackSegment segment, float t)
	{
        // Pobierz pozycję i rotację w punkcie 't' na segmencie
        // out = parametr wyjściowy (funkcja wypełnia te zmienne)
		Vector3 position;
		Quaternion rotation;
		segment.GetPointAt(t, out position, out rotation);
        
        // Załaduj prefab asynchronicznie przez Addressables
        // gameObject.name - nazwa prefaba (np. "TunnelObstacle")
        AsyncOperationHandle op = Addressables.InstantiateAsync(gameObject.name, position, rotation);
        
        // Poczekaj na zakończenie ładowania
        // yield return = pauza coroutine do zakończenia operacji
        yield return op;
        
        // Sprawdź czy ładowanie się powiodło
	    if (op.Result == null || !(op.Result is GameObject))
	    {
	        Debug.LogWarning(string.Format("Unable to load obstacle {0}.", gameObject.name));
	        yield break; // Przerwij coroutine - coś poszło nie tak
	    }
        
        // Rzutuj wynik na GameObject
        GameObject obj = op.Result as GameObject;
        
        // Ustaw segment jako rodzica przeszkody
        // true = zachowaj pozycję światową (world space)
        // Przeszkoda będzie "dzieckiem" segmentu w hierarchii
        obj.transform.SetParent(segment.objectRoot, true);

        // TODO : remove that hack related to #issue7
        // Workaround dla buga - przesuwamy i przywracamy pozycję
        // To "odświeża" transform po przypisaniu rodzica
        Vector3 oldPos = obj.transform.position;
        obj.transform.position += Vector3.back; // Przesuń o jednostkę w tył
        obj.transform.position = oldPos; // Przywróć oryginalną pozycję
    }
}
