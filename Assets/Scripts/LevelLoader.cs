using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// LevelLoader - Prosty skrypt do ładowania scen
/// 
/// DLA STUDENTÓW - ZARZĄDZANIE SCENAMI:
/// =====================================
/// 
/// Co to są sceny (Scenes)?
/// ------------------------
/// Scena to "poziom" lub "ekran" w grze Unity.
/// Każda scena ma własne GameObjecty, kamery, światła.
/// 
/// Przykłady scen:
/// - Menu główne
/// - Poziom 1
/// - Poziom 2
/// - Sklep
/// - Ekran ustawień
/// 
/// SceneManager:
/// ------------
/// To klasa Unity do zarządzania scenami.
/// Główne funkcje:
/// - LoadScene() - ładuje nową scenę (usuwa starą)
/// - LoadSceneAsync() - ładuje scenę w tle (nie blokuje gry)
/// - LoadSceneAdditively() - dodaje scenę do obecnej (obie działają)
/// - UnloadSceneAsync() - usuwa scenę addytywną
/// 
/// LoadScene() vs LoadSceneAsync():
/// -------------------------------
/// LoadScene() - BLOKUJE grę podczas ładowania
///   - Gra zamarznie na moment
///   - Proste, łatwe w użyciu
///   - OK dla małych scen
/// 
/// LoadSceneAsync() - ładuje W TLE
///   - Gra działa podczas ładowania
///   - Możesz pokazać pasek postępu
///   - Lepsze dla dużych scen
/// 
/// Użycie w tej grze:
/// -----------------
/// Ten skrypt jest używany przez przyciski do ładowania scen.
/// Podobnie jak OpenURL - podłączasz do przycisku UI.
/// 
/// Przykład:
/// - Przycisk "Play" wywołuje LoadLevel("game")
/// - Przycisk "Menu" wywołuje LoadLevel("menu")
/// 
/// WAŻNE - Build Settings:
/// ----------------------
/// Scena MUSI być dodana do Build Settings!
/// File > Build Settings > Add Open Scenes
/// 
/// Inaczej dostaniesz błąd:
/// "Scene '...' couldn't be loaded because it has not been added to the build settings"
/// </summary>
public class LevelLoader : MonoBehaviour
{
    /// <summary>
    /// LoadLevel - Ładuje scenę po nazwie
    /// 
    /// Parametr:
    /// ---------
    /// name - nazwa sceny do załadowania (np. "MainMenu", "Game", "Shop")
    /// 
    /// UWAGA:
    /// Nazwa MUSI dokładnie odpowiadać nazwie sceny w projekcie!
    /// Unity rozróżnia wielkie/małe litery: "Game" ≠ "game"
    /// 
    /// Co się dzieje:
    /// -------------
    /// 1. Unity zaczyna ładować nową scenę
    /// 2. Aktualna scena jest NISZCZONA (wszystkie GameObjecty)
    /// 3. Nowa scena jest ładowana
    /// 4. WYJĄTEK: Obiekty z DontDestroyOnLoad() przeżywają
    /// 
    /// Jak podłączyć do przycisku:
    /// --------------------------
    /// 1. Dodaj ten skrypt do jakiegoś GameObjecta w scenie
    /// 2. Na przycisku UI, w komponencie Button
    /// 3. On Click () -> kliknij "+"
    /// 4. Przeciągnij GameObject z tym skryptem
    /// 5. Wybierz LevelLoader -> LoadLevel
    /// 6. W polu tekstowym wpisz nazwę sceny (np. "MainMenu")
    /// 
    /// Alternatywa - LoadScene z kodem:
    /// --------------------------------
    /// Możesz też wywołać bezpośrednio w kodzie:
    /// 
    /// void OnButtonClick() {
    ///     SceneManager.LoadScene("MainMenu");
    /// }
    /// 
    /// Ten skrypt po prostu opakowuje to w funkcję którą łatwo
    /// podłączyć do przycisku w Inspectorze bez pisania kodu.
    /// </summary>
    public void LoadLevel(string name)
    {
        // Załaduj scenę o podanej nazwie
        // To zastępuje aktualną scenę - stara zostanie zniszczona
        SceneManager.LoadScene(name);
    }
}
