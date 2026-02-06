using System.Collections;
using UnityEngine;

/// <summary>
/// Obstacle - Klasa bazowa dla wszystkich przeszkód w grze
/// 
/// DLA STUDENTÓW - DZIEDZICZENIE I KLASY ABSTRAKCYJNE:
/// ===================================================
/// 
/// Co to jest klasa abstrakcyjna (abstract class)?
/// -----------------------------------------------
/// To "szablon" lub "wzór" dla innych klas.
/// NIE możesz stworzyć obiektu typu Obstacle bezpośrednio.
/// MUSISZ stworzyć klasę która dziedziczy po Obstacle.
/// 
/// Analogia ze świata rzeczywistego:
/// ---------------------------------
/// "Pojazd" to abstrakcja - nie możesz kupić "pojazdu".
/// Możesz kupić KONKRETNY pojazd: samochód, rower, motocykl.
/// 
/// W naszej grze:
/// - Obstacle = abstrakcja "przeszkoda"
/// - SimpleBarricade, Missile, AllLaneObstacle = konkretne przeszkody
/// 
/// Dlaczego tak robimy?
/// -------------------
/// 1. KOD WSPÓŁDZIELONY - wszystkie przeszkody potrzebują funkcji Impacted()
///    Zamiast kopiować kod do każdej przeszkody, piszemy raz tutaj
/// 
/// 2. POLIMORFIZM - możemy traktować wszystkie przeszkody jednakowo:
///    Obstacle obs = GetComponent<Obstacle>();  // Może być DOWOLNA przeszkoda
///    obs.Impacted();  // Zadziała dla każdej!
/// 
/// 3. WYMUSZENIE IMPLEMENTACJI - każda przeszkoda MUSI mieć funkcję Spawn()
///    Kompilator nie pozwoli stworzyć przeszkody bez tej funkcji
/// 
/// Typy funkcji:
/// ------------
/// 
/// virtual void Setup():
/// - "virtual" = funkcja może być NADPISANA w klasie pochodnej
/// - Ma domyślną implementację (pusta) ale możesz ją zmienić
/// - Przykład: SimpleBarricade może nie potrzebować Setup, ale Missile może
/// 
/// abstract IEnumerator Spawn():
/// - "abstract" = funkcja MUSI być zaimplementowana w klasie pochodnej
/// - Nie ma tutaj implementacji, tylko deklaracja
/// - Każda przeszkoda spawni się inaczej, więc każda musi to zdefiniować
/// 
/// virtual void Impacted():
/// - Wspólna logika dla wszystkich przeszkód (animacja + dźwięk)
/// - Możesz ją nadpisać jeśli potrzebujesz czegoś specjalnego
/// - Ale domyślnie zadziała dla każdej przeszkody
/// 
/// RequireComponent:
/// ----------------
/// Ten atrybut mówi Unity: "Ten skrypt WYMAGA komponentu AudioSource"
/// Jeśli dodasz Obstacle do GameObjecta bez AudioSource:
/// - Unity AUTOMATYCZNIE doda AudioSource
/// - Nie możesz zapomnieć dodać AudioSource
/// - Chroni przed błędami (NullReferenceException)
/// 
/// Przepływ w grze:
/// ---------------
/// 1. TrackManager tworzy segment toru
/// 2. Segment toru wywołuje Spawn() na przeszkodzie
/// 3. Przeszkoda się pojawia na torze
/// 4. Gracz uderza w przeszkodę
/// 5. CharacterCollider wywołuje Impacted()
/// 6. Gra animacja i dźwięk uderzenia
/// </summary>
[RequireComponent(typeof(AudioSource))]  // Automatycznie dodaje AudioSource jeśli nie ma
public abstract class Obstacle : MonoBehaviour
{
    // Dźwięk odtwarzany gdy gracz uderzy w przeszkodę
    // Ustawiany w Inspectorze Unity dla każdego prefaba przeszkody
	public AudioClip impactedSound;

    /// <summary>
    /// Setup - Inicjalizacja przeszkody (opcjonalna)
    /// 
    /// "virtual" oznacza że klasy pochodne MOGĄ nadpisać tę funkcję.
    /// Domyślnie nic nie robi (pusta implementacja).
    /// 
    /// Przykłady użycia:
    /// - Missile mogłaby tutaj ustawić prędkość lotu
    /// - PatrollingObstacle mogłaby ustawić tor patrolu
    /// - SimpleBarricade prawdopodobnie nie potrzebuje Setup
    /// 
    /// Wywołanie:
    /// Zwykle wywoływana gdy przeszkoda jest dodawana do puli (pooling).
    /// </summary>
    public virtual void Setup() {}

    /// <summary>
    /// Spawn - Pojawienie się przeszkody na torze
    /// 
    /// "abstract" oznacza że KAŻDA klasa pochodna MUSI zaimplementować tę funkcję.
    /// Nie ma tutaj implementacji - każda przeszkoda spawni się inaczej.
    /// 
    /// IEnumerator = Coroutine:
    /// -----------------------
    /// Funkcja może rozłożyć spawning na wiele klatek.
    /// Przydatne dla animacji pojawiania się.
    /// 
    /// Parametry:
    /// ----------
    /// segment - segment toru na którym się spawnimy
    /// t - pozycja na segmencie (0.0 = początek, 1.0 = koniec)
    /// 
    /// Przykładowa implementacja w SimpleBarricade:
    /// 
    /// public override IEnumerator Spawn(TrackSegment segment, float t) {
    ///     transform.position = segment.GetPointAt(t);
    ///     gameObject.SetActive(true);
    ///     yield break;  // Koniec coroutine
    /// }
    /// </summary>
    public abstract IEnumerator Spawn(TrackSegment segment, float t);

    /// <summary>
    /// Impacted - Wywoływana gdy gracz uderzy w przeszkodę
    /// 
    /// "virtual" = można nadpisać ale nie trzeba.
    /// Ta implementacja działa dla większości przeszkód.
    /// 
    /// Co się dzieje:
    /// -------------
    /// 1. Szukamy komponentu Animation w dzieciach (animacja uderzenia)
    /// 2. Pobieramy AudioSource (wymagany przez RequireComponent)
    /// 3. Jeśli jest animacja - odtwarzamy ją
    /// 4. Jeśli jest dźwięk - odtwarzamy go
    /// 
    /// GetComponentInChildren vs GetComponent:
    /// ---------------------------------------
    /// GetComponent - szuka TYLKO na tym GameObjeccie
    /// GetComponentInChildren - szuka na tym GameObjeccie I wszystkich dzieciach
    /// 
    /// Często animacja jest na dziecku (child), nie na głównym obiekcie.
    /// Przykład hierarchii:
    /// 
    /// Barricade (GameObject) <- Obstacle script
    ///   └─ Visual (GameObject) <- Animation component
    /// 
    /// AudioSource.Stop() + Play():
    /// ---------------------------
    /// Stop() - zatrzymuje aktualny dźwięk (gdyby coś grało)
    /// loop = false - dźwięk zagra raz i się zatrzyma
    /// clip = impactedSound - ustaw który dźwięk grać
    /// Play() - odtwórz dźwięk
    /// 
    /// Dlaczego Stop() przed Play()?
    /// Gdyby AudioSource już coś odtwarzał, chcemy to przerwać i
    /// zagrać dźwięk uderzenia NATYCHMIAST.
    /// </summary>
	public virtual void Impacted()
	{
        // Szukaj komponentu Animation w tym obiekcie lub jego dzieciach
        // Animation to stary system animacji Unity (przed Animator)
		Animation anim = GetComponentInChildren<Animation>();
        
        // Pobierz AudioSource - MUSI istnieć (RequireComponent wymusza to)
		AudioSource audioSource = GetComponent<AudioSource>();

        // Jeśli znaleziono animację, odtwórz ją
		if (anim != null)
		{
            // Play() odtwarza domyślną animację przypisaną do Animation
			anim.Play();
		}

        // Jeśli mamy AudioSource i przypisany dźwięk uderzenia
		if (audioSource != null && impactedSound != null)
		{
            // Zatrzymaj aktualny dźwięk (jeśli coś gra)
			audioSource.Stop();
            // Nie zapętlaj dźwięku - zagraj raz
			audioSource.loop = false;
            // Ustaw który klip audio odtworzyć
			audioSource.clip = impactedSound;
            // Odtwórz dźwięk
			audioSource.Play();
		}
	}
}
