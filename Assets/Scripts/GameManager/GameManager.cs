using UnityEngine;
using System.Collections.Generic;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

/// <summary>
/// GameManager - Główny manager gry oparty na maszynie stanów
/// 
/// DLA STUDENTÓW:
/// Ten skrypt jest SERCEM całej gry. To tutaj kontrolujemy w jakim "stanie" znajduje się gra.
/// 
/// Co to jest "stan" (state)?
/// - Stan to tryb, w którym znajduje się gra (np. Menu, Gra, GameOver)
/// - W każdym stanie gra zachowuje się inaczej
/// - Tylko JEDEN stan może być aktywny naraz (ten na górze stosu)
/// 
/// Wzorzec State Machine (Maszyna Stanów):
/// - To popularny wzorzec projektowy w programowaniu gier
/// - Pozwala łatwo zarządzać tym, co gra robi w różnych sytuacjach
/// - Zamiast jednego gigantycznego skryptu z ifami, mamy osobne stany
/// 
/// Jak to działa?
/// 1. Mamy listę dostępnych stanów (states)
/// 2. Mamy stos aktywnych stanów (m_StateStack) - jak stos talerzy
/// 3. Aktywny stan jest zawsze na górze stosu (topState)
/// 4. Co każdą klatkę (Update) wywołujemy funkcję Tick() aktywnego stanu
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton - zapewnia że istnieje tylko jedna instancja GameManagera w grze
    // Możemy odwołać się do niego z każdego miejsca poprzez GameManager.instance
    static public GameManager instance { get { return s_Instance; } }
    static protected GameManager s_Instance;

    // Tablica wszystkich możliwych stanów w grze (ustawiana w Inspektorze Unity)
    public AState[] states;
    
    // Właściwość zwracająca aktywny stan (ten na szczycie stosu)
    // Jeśli stos jest pusty, zwraca null
    public AState topState {  get { if (m_StateStack.Count == 0) return null; return m_StateStack[m_StateStack.Count - 1]; } }

    // Baza danych wszystkich przedmiotów zużywalnych (power-upy, monety etc.)
    public ConsumableDatabase m_ConsumableDatabase;

    // Stos stanów - aktywny stan jest na górze (ostatni element listy)
    protected List<AState> m_StateStack = new List<AState>();
    
    // Słownik stanów - pozwala szybko znaleźć stan po nazwie
    // Zamiast przeszukiwać całą tablicę, po prostu używamy nazwy jako klucza
    protected Dictionary<string, AState> m_StateDict = new Dictionary<string, AState>();

    /// <summary>
    /// OnEnable - wywoływana gdy obiekt jest włączany
    /// Tutaj inicjalizujemy grę i ustawiamy pierwszy stan
    /// </summary>
    protected void OnEnable()
    {
        // Tworzymy/ładujemy dane gracza (wyniki, monety, odblokowane postacie)
        PlayerData.Create();

        // Ustawiamy singleton - teraz możemy używać GameManager.instance
        s_Instance = this;

        // Ładujemy bazę danych przedmiotów zużywalnych
        m_ConsumableDatabase.Load();

        // Budujemy słownik stanów dla łatwego przełączania się między nimi po nazwie
        // Dlaczego słownik? Bo wyszukiwanie po kluczu jest O(1), czyli bardzo szybkie!
        m_StateDict.Clear();

        // Jeśli nie ma żadnych stanów, kończymy - nie ma czego inicjalizować
        if (states.Length == 0)
            return;

        // Przechodzimy przez wszystkie stany i dodajemy je do słownika
        for(int i = 0; i < states.Length; ++i)
        {
            // Każdy stan musi wiedzieć kto jest jego managerem (czyli my)
            states[i].manager = this;
            
            // Dodajemy stan do słownika używając jego nazwy jako klucza
            m_StateDict.Add(states[i].GetName(), states[i]);
        }

        // Czyścimy stos stanów (na wszelki wypadek)
        m_StateStack.Clear();

        // Uruchamiamy pierwszy stan z tablicy (zwykle LoadoutState - menu główne)
        PushState(states[0].GetName());
    }

    /// <summary>
    /// Update - wywoływany co każdą klatkę (frame)
    /// Tutaj aktualizujemy aktywny stan
    /// </summary>
    protected void Update()
    {
        // Jeśli mamy jakiś aktywny stan na stosie
        if(m_StateStack.Count > 0)
        {
            // Wywołaj jego funkcję Tick() - tam znajduje się logika tego stanu
            // m_StateStack.Count - 1 to indeks ostatniego elementu (góra stosu)
            m_StateStack[m_StateStack.Count - 1].Tick();
        }
    }

    /// <summary>
    /// OnApplicationQuit - wywoływana gdy gracz zamyka grę
    /// UWAGA: Ta funkcja NIE jest wywoływana na urządzeniach mobilnych!
    /// Działa tylko na PC/Mac/Linux
    /// </summary>
    protected void OnApplicationQuit()
    {
#if UNITY_ANALYTICS
        // Jeśli mamy włączoną analitykę Unity, logujemy zdarzenie wyjścia z gry
        // Sprawdzamy czy gracz wyszedł podczas rozgrywki (to ważne dla statystyk)
        bool inGameExit = m_StateStack[m_StateStack.Count - 1].GetType() == typeof(GameState);

        // Wysyłamy zdarzenie do Unity Analytics
        Analytics.CustomEvent("user_end_session", new Dictionary<string, object>
        {
            { "force_exit", inGameExit },  // Czy wyszedł podczas gry?
            { "timer", Time.realtimeSinceStartup }  // Ile czasu spędził w grze?
        });
#endif
    }

    /// <summary>
    /// SwitchState - ZAMIENIA aktywny stan na nowy
    /// Różnica od Push: stary stan jest USUWANY ze stosu
    /// 
    /// Kiedy używać?
    /// - Gdy całkowicie przechodzimy do nowego stanu (np. z Menu do Gry)
    /// - Gdy nie chcemy wracać do poprzedniego stanu
    /// 
    /// Co się dzieje?
    /// 1. Znajdujemy nowy stan po nazwie
    /// 2. Wywołujemy Exit() na starym stanie (sprzątanie)
    /// 3. Wywołujemy Enter() na nowym stanie (inicjalizacja)
    /// 4. Usuwamy stary stan ze stosu
    /// 5. Dodajemy nowy stan na stos
    /// </summary>
    public void SwitchState(string newState)
    {
        // Znajdź stan o podanej nazwie w słowniku
        AState state = FindState(newState);
        if (state == null)
        {
            Debug.LogError("Can't find the state named " + newState);
            return;
        }

        // Informujemy stary stan że z niego wychodzimy
        m_StateStack[m_StateStack.Count - 1].Exit(state);
        
        // Informujemy nowy stan że do niego wchodzimy
        state.Enter(m_StateStack[m_StateStack.Count - 1]);
        
        // Usuwamy stary stan ze stosu
        m_StateStack.RemoveAt(m_StateStack.Count - 1);
        
        // Dodajemy nowy stan na stos
        m_StateStack.Add(state);
    }

    /// <summary>
    /// FindState - pomocnicza funkcja do znajdowania stanu po nazwie
    /// Zwraca null jeśli stan nie istnieje
    /// </summary>
	public AState FindState(string stateName)
	{
		AState state;
		// TryGetValue to bezpieczny sposób pobierania wartości ze słownika
		// Zwraca true jeśli klucz istnieje, false jeśli nie
		if (!m_StateDict.TryGetValue(stateName, out state))
		{
			return null;
		}

		return state;
	}

    /// <summary>
    /// PopState - USUWA stan z góry stosu i wraca do poprzedniego
    /// 
    /// To jak przycisk "Wstecz" w grze!
    /// 
    /// Kiedy używać?
    /// - Gdy zamykamy tymczasowe okno (np. sklep)
    /// - Gdy chcemy wrócić do poprzedniego stanu
    /// 
    /// Przykład:
    /// Stos przed: [LoadoutState, ShopState]
    /// Po PopState: [LoadoutState]
    /// 
    /// UWAGA: Muszą być przynajmniej 2 stany na stosie!
    /// </summary>
    public void PopState()
    {
        // Sprawdzamy czy mamy przynajmniej 2 stany
        if(m_StateStack.Count < 2)
        {
            Debug.LogError("Can't pop states, only one in stack.");
            return;
        }

        // Wywołujemy Exit na aktualnym stanie (góra stosu)
        m_StateStack[m_StateStack.Count - 1].Exit(m_StateStack[m_StateStack.Count - 2]);
        
        // Wywołujemy Enter na poprzednim stanie (wracamy do niego)
        m_StateStack[m_StateStack.Count - 2].Enter(m_StateStack[m_StateStack.Count - 2]);
        
        // Usuwamy aktualny stan ze stosu
        m_StateStack.RemoveAt(m_StateStack.Count - 1);
    }

    /// <summary>
    /// PushState - DODAJE nowy stan NA WIERZCH stosu
    /// Poprzedni stan pozostaje na stosie, ale jest nieaktywny
    /// 
    /// To jak otwieranie nowego okna - poprzednie nadal jest "pod spodem"
    /// 
    /// Kiedy używać?
    /// - Gdy otwieramy tymczasowe okno (GameOver, Shop)
    /// - Gdy chcemy móc wrócić do poprzedniego stanu
    /// 
    /// Przykład:
    /// Stos przed: [LoadoutState]
    /// Po PushState("Shop"): [LoadoutState, ShopState]
    /// LoadoutState nadal istnieje, ale ShopState jest teraz aktywny
    /// </summary>
    public void PushState(string name)
    {
        AState state;
        // Szukamy stanu w słowniku
        if(!m_StateDict.TryGetValue(name, out state))
        {
            Debug.LogError("Can't find the state named " + name);
            return;
        }

        // Jeśli mamy już jakiś stan na stosie
        if (m_StateStack.Count > 0)
        {
            // Informujemy go że tymczasowo z niego wychodzimy
            m_StateStack[m_StateStack.Count - 1].Exit(state);
            // Informujemy nowy stan że do niego wchodzimy
            state.Enter(m_StateStack[m_StateStack.Count - 1]);
        }
        else
        {
            // Jeśli to pierwszy stan, nie ma poprzedniego
            state.Enter(null);
        }
        
        // Dodajemy nowy stan na górę stosu
        m_StateStack.Add(state);
    }
}

/// <summary>
/// AState - abstrakcyjna klasa bazowa dla wszystkich stanów gry
/// 
/// DLA STUDENTÓW:
/// "abstract class" to szablon, wzór dla innych klas
/// Nie możesz stworzyć obiektu AState - musisz stworzyć klasę która dziedziczy po AState
/// 
/// Każdy stan w grze (LoadoutState, GameState, GameOverState) dziedziczy po tej klasie
/// i MUSI zaimplementować wszystkie abstract funkcje
/// 
/// Dlaczego to jest przydatne?
/// - Każdy stan MA funkcje: Enter, Exit, Tick, GetName
/// - GameManager wie że może wywołać te funkcje na każdym stanie
/// - Nie musi wiedzieć jaki to konkretnie stan!
/// 
/// To jest polimorfizm - jedna z fundamentalnych koncepcji programowania obiektowego
/// </summary>
public abstract class AState : MonoBehaviour
{
    // Referencja do GameManagera - każdy stan musi wiedzieć kto nim zarządza
    [HideInInspector]  // Ukryj w Inspectorze - to pole jest ustawiane automatycznie
    public GameManager manager;

    /// <summary>
    /// Enter - wywoływana gdy WCHODZIMY do tego stanu
    /// Tutaj inicjalizujemy wszystko co potrzebne dla tego stanu
    /// 
    /// Parametr 'from': z jakiego stanu przyszliśmy (może być null jeśli to pierwszy stan)
    /// </summary>
    public abstract void Enter(AState from);
    
    /// <summary>
    /// Exit - wywoływana gdy WYCHODZIMY z tego stanu
    /// Tutaj sprzątamy, wyłączamy co trzeba
    /// 
    /// Parametr 'to': do jakiego stanu przechodzimy
    /// </summary>
    public abstract void Exit(AState to);
    
    /// <summary>
    /// Tick - wywoływana CO KLATKĘ gdy ten stan jest aktywny
    /// Tutaj znajduje się główna logika stanu
    /// 
    /// To odpowiednik Update() dla stanu
    /// </summary>
    public abstract void Tick();

    /// <summary>
    /// GetName - zwraca nazwę stanu (np. "Game", "Loadout", "GameOver")
    /// Używane przez GameManager do identyfikacji stanów
    /// </summary>
    public abstract string GetName();
}