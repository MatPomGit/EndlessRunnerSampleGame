using UnityEngine;

/// <summary>
/// WorldCurver - Zakrzywienie świata w dół
/// 
/// DLA STUDENTÓW - GRAFIKA I SHADERY:
/// ===================================
/// 
/// Problem w grach Endless Runner:
/// --------------------------------
/// W grach typu endless runner (nieskończony biegacz) tor ciągnie się w nieskończoność.
/// Ale jest kilka problemów:
/// 
/// 1. HORYZONT - gracz widzi "koniec świata" w oddali (brzydkie!)
/// 2. POPPING - nowe segmenty toru nagle "pojawiają się" (pop!) w oddali
/// 3. BRAK GŁĘBI - płaski tor wygląda nudno
/// 
/// Rozwiązanie - Zakrzywienie w dół:
/// ---------------------------------
/// Zakrzywiamy cały świat w dół (jakby Ziemia była NAPRAWDĘ okrągła).
/// 
/// Wyobraź sobie:
/// - Normalny tor: _____________________ (płaski, widać koniec)
/// - Zakrzywiony tor: ___________        (zakrzywia się w dół, koniec ukryty)
///                              \___
/// 
/// Korzyści:
/// --------
/// 1. Horyzont jest UKRYTY (zakrzywienie schodzi poniżej kamery)
/// 2. Nowe segmenty pojawiają się "zza horyzontu" (naturalnie!)
/// 3. Wrażenie większej odległości i głębi
/// 4. Wygląda ŚWIETNIE! (jak w Subway Surfers, Temple Run)
/// 
/// Jak to działa TECHNICZNIE?
/// --------------------------
/// To NIE jest fizyczne zakrzywienie geometrii (wierzchołków).
/// To SHADER - program który modyfikuje pozycję wierzchołków podczas renderowania.
/// 
/// Shader vs Skrypt:
/// ----------------
/// SKRYPT (CPU): Moglibyśmy zmienić pozycje wierzchołków w C#, ale:
///   - WOLNE (CPU musi przeliczyć KAŻDY wierzchołek każdej klatki)
///   - TYSIĄCE wierzchołków = spadek FPS
/// 
/// SHADER (GPU): Karta graficzna robi to RÓWNOLEGLE:
///   - SZYBKIE (GPU ma SETKI rdzeni, przetwarza wiele wierzchołków naraz)
///   - Brak wpływu na FPS
/// 
/// ExecuteInEditMode:
/// ------------------
/// Ten atrybut sprawia że skrypt działa TAKŻE w edytorze (nie tylko w grze).
/// Dzięki temu możesz regulować curveStrength w edytorze i OD RAZU widzieć efekt!
/// Normalnie skrypty działają tylko gdy gra jest uruchomiona (Play mode).
/// 
/// Shader.SetGlobalFloat:
/// ---------------------
/// "Global" oznacza że ta wartość jest dostępna dla WSZYSTKICH shaderów w grze.
/// Każdy obiekt który używa shadera z parametrem "_CurveStrength" dostanie tę wartość.
/// 
/// To lepsze niż ustawianie dla każdego obiektu osobno:
/// - WYDAJNOŚĆ - jedna operacja zamiast setek
/// - SPÓJNOŚĆ - wszystkie obiekty mają tę samą siłę zakrzywienia
/// - ŁATWOŚĆ - możemy zmienić wszystko z jednego miejsca
/// 
/// Shader.PropertyToID:
/// -------------------
/// Shadery mają parametry (jak zmienne).
/// PropertyToID konwertuje nazwę (string) na ID (int).
/// 
/// Dlaczego?
/// - Porównywanie int jest SZYBSZE niż porównywanie string
/// - Robimy to raz w OnEnable, potem używamy szybkiego int
/// 
/// Zamiast:
///   Shader.SetGlobalFloat("_CurveStrength", value);  // Wolne, string
/// Robimy:
///   int id = Shader.PropertyToID("_CurveStrength");  // Raz, na starcie
///   Shader.SetGlobalFloat(id, value);  // Szybkie, każdą klatkę
/// 
/// Range [−0.1f, 0.1f]:
/// -------------------
/// Ten atrybut tworzy SLIDER w Inspectorze Unity.
/// Możesz przeciągać suwak zamiast wpisywać liczby.
/// - Wartości ujemne: zakrzywienie w GÓRĘ
/// - Wartości dodatnie: zakrzywienie w DÓŁ
/// - 0: brak zakrzywienia (płasko)
/// 
/// Typowa wartość: 0.01 (delikatne zakrzywienie w dół)
/// </summary>
[ExecuteInEditMode]  // Skrypt działa także w trybie edycji (nie tylko w Play mode)
public class WorldCurver : MonoBehaviour
{
    // Siła zakrzywienia świata
    // Ujemne wartości = zakrzywienie w górę
    // Dodatnie wartości = zakrzywienie w dół (używane w grze)
    // 0 = brak zakrzywienia
	[Range(-0.1f, 0.1f)]
	public float curveStrength = 0.01f;

    // ID właściwości shadera (int jest szybszy niż string)
    // Przechowujemy to zamiast używać stringa co klatkę
    int m_CurveStrengthID;

    /// <summary>
    /// OnEnable - wywoływane gdy skrypt jest włączany
    /// 
    /// Konwertujemy nazwę parametru shadera na ID (int).
    /// Robimy to RAZ tutaj, zamiast co klatkę w Update.
    /// Optymalizacja: int jest szybszy od string.
    /// </summary>
    private void OnEnable()
    {
        // Konwertuj string "_CurveStrength" na int ID
        // Ten ID będzie używany w Update do ustawiania wartości
        m_CurveStrengthID = Shader.PropertyToID("_CurveStrength");
    }

    /// <summary>
    /// Update - wywoływane co klatkę
    /// 
    /// Ustawia globalną wartość shadera dla całej sceny.
    /// WSZYSTKIE shadery które używają parametru "_CurveStrength"
    /// dostaną tę wartość.
    /// 
    /// Dlaczego co klatkę?
    /// ------------------
    /// 1. Możemy zmieniać curveStrength dynamicznie (np. efekty specjalne)
    /// 2. W edytorze: możesz zmieniać slider i NATYCHMIAST widzieć efekt
    /// 3. Shader.SetGlobal... jest bardzo szybkie, brak wpływu na FPS
    /// </summary>
	void Update()
	{
        // Ustaw globalną wartość shadera
        // m_CurveStrengthID - szybki int zamiast powolnego stringa
        // curveStrength - aktualna wartość (możesz ją zmieniać w Inspectorze!)
		Shader.SetGlobalFloat(m_CurveStrengthID, curveStrength);
	}
}
