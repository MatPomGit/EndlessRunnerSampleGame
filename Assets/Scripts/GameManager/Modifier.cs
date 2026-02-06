using UnityEngine;

/// <summary>
/// Modifier - Klasa bazowa do modyfikowania zasad gry
/// 
/// DLA STUDENTÓW - WZORZEC STRATEGII (Strategy Pattern):
/// ======================================================
/// 
/// Problem:
/// --------
/// Czasami chcemy zmienić zasady gry bez przepisywania całego GameState.
/// 
/// Przykłady:
/// - Tryb wyzwania: biegnij tylko 500m
/// - Tryb tygodniowy: wszyscy mają tę samą mapę (ten sam seed)
/// - Tryb hardcore: tylko jedno życie
/// 
/// Naiwne rozwiązanie (ZŁE):
/// -------------------------
/// if (challengeMode) {
///     // specjalna logika
/// } else if (weeklyMode) {
///     // inna specjalna logika
/// } else if (hardcoreMode) {
///     // jeszcze inna logika
/// }
/// 
/// Problem: GameState staje się GIGANTYCZNY, pełen if-ów!
/// 
/// Rozwiązanie - Wzorzec Strategii:
/// --------------------------------
/// Zamiast if-ów, mamy OSOBNE klasy dla każdego trybu.
/// GameState po prostu wywołuje metody na aktywnym Modifierze.
/// 
/// Jak to działa:
/// -------------
/// 1. GameState ma zmienną 'modifier' typu Modifier
/// 2. W zależności od trybu, ustawiamy odpowiedni Modifier:
///    - modifier = new LimitedLengthRun(500); // challenge
///    - modifier = new SeededRun(); // weekly
///    - modifier = new SingleLifeRun(); // hardcore
/// 3. GameState wywołuje:
///    - modifier.OnRunStart(this) - gdy rozgrywka się zaczyna
///    - modifier.OnRunTick(this) - co klatkę podczas gry
///    - modifier.OnRunEnd(this) - gdy rozgrywka się kończy
/// 4. Każdy Modifier robi co innego!
/// 
/// Korzyści:
/// --------
/// - GameState jest PROSTY - tylko wywołuje metody
/// - Łatwo dodać NOWY tryb - stworzyć nową klasę Modifier
/// - Modular code - każdy tryb w osobnym pliku
/// - Testowanie - możesz testować każdy tryb osobno
/// 
/// Funkcje wirtualne:
/// -----------------
/// "virtual" oznacza że klasy pochodne MOGĄ nadpisać metodę.
/// Domyślnie metody nic nie robią (puste).
/// 
/// Tylko nadpisujesz te metody których potrzebujesz:
/// - LimitedLengthRun nadpisuje OnRunTick (sprawdza dystans)
/// - SeededRun nadpisuje OnRunStart (ustawia seed)
/// - SingleLifeRun nadpisuje OnRunTick (ogranicza życia)
/// 
/// Parametr 'state':
/// ----------------
/// Każda metoda dostaje referencję do GameState.
/// Dzięki temu Modifier może:
/// - Czytać dane: state.trackManager.worldDistance
/// - Modyfikować grę: state.trackManager.characterController.currentLife = 0
/// - Zmieniać stan: state.QuitToLoadout()
/// 
/// OnRunEnd - wartość zwracana:
/// ---------------------------
/// bool = czy pokazać ekran Game Over?
/// - true: pokaż Game Over (normalny tryb)
/// - false: wróć od razu do menu (tryby challenge/weekly)
/// </summary>
public class Modifier
{
    /// <summary>
    /// OnRunStart - Wywoływana gdy rozgrywka się zaczyna
    /// 
    /// Użyj do:
    /// - Ustawienia początkowych parametrów
    /// - Inicjalizacji liczników
    /// - Ustawienia seeda dla generatora
    /// 
    /// Przykład: SeededRun ustawia seed tutaj
    /// </summary>
	public virtual void OnRunStart(GameState state)
	{
        // Domyślnie nic nie robi - nadpisz w klasie pochodnej jeśli potrzebujesz
	}

    /// <summary>
    /// OnRunTick - Wywoływana CO KLATKĘ podczas rozgrywki
    /// 
    /// UWAGA: To jest wywoływane BARDZO CZĘSTO (60+ razy na sekundę)!
    /// Nie rób tu ciężkich obliczeń.
    /// 
    /// Użyj do:
    /// - Sprawdzania warunków (czy osiągnięto limit?)
    /// - Wymuszania zasad (maksymalnie 1 życie)
    /// - Aktualizacji liczników
    /// 
    /// Przykłady:
    /// - LimitedLengthRun sprawdza czy dystans >= limit
    /// - SingleLifeRun wymusza życia <= 1
    /// </summary>
	public virtual void OnRunTick(GameState state)
	{
        // Domyślnie nic nie robi - nadpisz w klasie pochodnej jeśli potrzebujesz
	}

    /// <summary>
    /// OnRunEnd - Wywoływana gdy rozgrywka się kończy (gracz przegrał)
    /// 
    /// return true = pokaż ekran Game Over
    /// return false = wróć od razu do menu (bez Game Over)
    /// 
    /// Użyj do:
    /// - Zapisania wyniku challenge
    /// - Wysłania statystyk na serwer
    /// - Decydowania czy pokazać Game Over
    /// 
    /// Przykład:
    /// Tryby challenge/weekly zwracają false (bez Game Over screen)
    /// bo chcą wrócić od razu do menu.
    /// </summary>
	public virtual bool OnRunEnd(GameState state)
	{
        // Domyślnie pokaż Game Over
		return true;
	}
}

// Poniżej znajdują się przykładowe Modifiery używane w grze:

/// <summary>
/// LimitedLengthRun - Tryb z ograniczoną długością biegu
/// 
/// Używany w trybie CHALLENGE.
/// Gracz musi przebiec określony dystans - nie więcej, nie mniej.
/// 
/// Jak działa:
/// -----------
/// - OnRunTick sprawdza co klatkę dystans
/// - Gdy worldDistance >= limit, ustawia życia na 0 (koniec gry)
/// - OnRunEnd wraca od razu do menu (bez Game Over screen)
/// 
/// Przykład użycia:
/// ---------------
/// Modifier mod = new LimitedLengthRun(500f); // Challenge: przebiegnij 500m
/// </summary>
public class LimitedLengthRun : Modifier
{
    // Dystans który gracz musi przebiec
	public float distance;

    /// <summary>
    /// Konstruktor - tworzy challenge z określonym dystansem
    /// </summary>
	public LimitedLengthRun(float dist)
	{
		distance = dist;
	}

    /// <summary>
    /// Sprawdza co klatkę czy gracz osiągnął limit dystansu
    /// Jeśli tak, kończy grę (życia = 0)
    /// </summary>
	public override void OnRunTick(GameState state)
	{
        // worldDistance = całkowity przebiegły dystans
		if(state.trackManager.worldDistance >= distance)
		{
            // Koniec gry - ustaw życia na 0
			state.trackManager.characterController.currentLife = 0;
		}
	}

	public override void OnRunStart(GameState state)
	{
        // Nie potrzebujemy specjalnej inicjalizacji
	}

    /// <summary>
    /// Wraca od razu do menu (bez ekranu Game Over)
    /// return false = nie pokazuj Game Over screen
    /// </summary>
	public override bool OnRunEnd(GameState state)
	{
		state.QuitToLoadout(); // Wróć do menu głównego
		return false; // NIE pokazuj Game Over
	}
}

/// <summary>
/// SeededRun - Tryb z ustalonym seedem (ta sama mapa dla wszystkich)
/// 
/// Używany w trybie WEEKLY CHALLENGE.
/// Wszyscy gracze mają TĘ SAMĄ mapę (przeszkody w tych samych miejscach).
/// 
/// Seed:
/// -----
/// Seed to "ziarno" dla generatora losowego.
/// Ten sam seed = zawsze ta sama "losowość".
/// 
/// Analogia:
/// Wyobraź sobie książkę przepisów (seed = numer przepisu).
/// Jeśli dwóch kucharzy użyje tego samego przepisu (seed),
/// dostaną takie same ciasto (mapa).
/// 
/// Seed tygodniowy:
/// ---------------
/// Seed zmienia się co TYDZIEŃ (7 dni).
/// DayOfYear / 7 = numer tygodnia w roku
/// 
/// Przykład:
/// - 1 stycznia (dzień 1): 1 / 7 = 0 (seed = 0)
/// - 8 stycznia (dzień 8): 8 / 7 = 1 (seed = 1)
/// 
/// Wszyscy gracze grający w tym samym tygodniu mają ten sam seed!
/// </summary>
public class SeededRun : Modifier
{
	int m_Seed; // Seed dla generatora losowego

    // Stała: liczba dni w tygodniu
    protected const int k_DaysInAWeek = 7;

    /// <summary>
    /// Konstruktor - oblicza seed na podstawie aktualnego tygodnia
    /// </summary>
	public SeededRun()
	{
        // DateTime.Now.DayOfYear = dzień roku (1-365)
        // Podziel przez 7 = numer tygodnia
        // Wszyscy gracze w tym samym tygodniu dostaną ten sam seed!
        m_Seed = System.DateTime.Now.DayOfYear / k_DaysInAWeek;
	}

    /// <summary>
    /// Ustawia seed w TrackManagerze
    /// Od tej pory TrackManager będzie generować zawsze tę samą mapę
    /// </summary>
	public override void OnRunStart(GameState state)
	{
		state.trackManager.trackSeed = m_Seed;
	}

    /// <summary>
    /// Wraca od razu do menu (bez ekranu Game Over)
    /// </summary>
	public override bool OnRunEnd(GameState state)
	{
		state.QuitToLoadout();
		return false; // NIE pokazuj Game Over
	}
}

/// <summary>
/// SingleLifeRun - Tryb z jednym życiem (hardcore!)
/// 
/// Normalnie gracz ma 3 życia.
/// W tym trybie ZAWSZE ma MAKSYMALNIE 1 życie.
/// 
/// Jak działa:
/// -----------
/// OnRunTick sprawdza co klatkę życia.
/// Jeśli życia > 1, wymusza życia = 1.
/// 
/// To zapobiega oszukiwaniu:
/// - Gracz nie może użyć power-upu Extra Life
/// - Gracz nie może obejrzeć reklamy za życie
/// - ZAWSZE maksymalnie 1 życie!
/// 
/// Trudność:
/// --------
/// Jedna pomyłka = koniec gry!
/// To sprawia że tryb jest BARDZO trudny.
/// </summary>
public class SingleLifeRun : Modifier
{
    /// <summary>
    /// Wymusza maksymalnie 1 życie
    /// Wywoływane co klatkę - nie może być oszukane!
    /// </summary>
	public override void OnRunTick(GameState state)
	{
        // Jeśli gracz jakoś ma więcej niż 1 życie
		if (state.trackManager.characterController.currentLife > 1)
            // Wymuszaj 1 życie
			state.trackManager.characterController.currentLife = 1;
	}

	public override void OnRunStart(GameState state)
	{
        // Nie potrzebujemy specjalnej inicjalizacji
	}

    /// <summary>
    /// Wraca od razu do menu (bez ekranu Game Over)
    /// </summary>
	public override bool OnRunEnd(GameState state)
	{
		state.QuitToLoadout();
		return false; // NIE pokazuj Game Over
	}
}
