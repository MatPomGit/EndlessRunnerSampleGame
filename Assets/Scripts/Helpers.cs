using UnityEngine;

/// <summary>
/// Helpers - Klasa pomocnicza z użytecznymi funkcjami narzędziowymi
/// 
/// DLA STUDENTÓW:
/// ==============
/// Klasy "Helper" lub "Utility" to powszechny wzorzec w programowaniu.
/// Przechowują funkcje, które są używane w wielu miejscach, ale nie pasują
/// do żadnej konkretnej klasy.
/// 
/// Dlaczego wszystkie funkcje są "static"?
/// ----------------------------------------
/// Static oznacza że:
/// 1. NIE musisz tworzyć obiektu Helpers
/// 2. Wywołujesz funkcje bezpośrednio: Helpers.SetRendererLayerRecursive(...)
/// 3. To jak funkcje "globalne" (choć to nie do końca prawda w C#)
/// 
/// Kiedy używać static?
/// - Gdy funkcja nie potrzebuje danych obiektu (nie ma zmiennych członkowskich)
/// - Gdy funkcja to po prostu "narzędzie" do obliczeń/transformacji
/// 
/// Przykłady w Unity:
/// - Mathf.Sin(), Mathf.Clamp() - wszystkie static
/// - Physics.Raycast() - static
/// - GameObject.Find() - static
/// </summary>
public class Helpers
{
    /// <summary>
    /// SetRendererLayerRecursive - Ustawia warstwę (layer) dla wszystkich rendererów w hierarchii
    /// 
    /// DLA STUDENTÓW - Layers w Unity:
    /// ================================
    /// 
    /// Co to są Layers?
    /// ----------------
    /// Layers (warstwy) to sposób na kategoryzowanie obiektów w Unity.
    /// Każdy GameObject ma przypisany JEDEN layer (np. "Default", "UI", "Water").
    /// 
    /// Do czego się to przydaje?
    /// -------------------------
    /// 1. KAMERY - mogą renderować tylko wybrane warstwy
    ///    Przykład: Kamera UI renderuje tylko layer "UI"
    ///    
    /// 2. FIZYKA - możesz wyłączyć kolizje między warstwami
    ///    Przykład: Layer "Player" nie koliduje z layerem "PlayerProjectile"
    ///    
    /// 3. RAYCASTY - możesz robić raycast tylko na wybranych warstwach
    ///    Przykład: Sprawdzanie kliknięcia tylko na layerze "Clickable"
    /// 
    /// Dlaczego potrzebujemy tej funkcji?
    /// ----------------------------------
    /// W grze często mamy obiekt z wieloma dziećmi (hierarchia).
    /// Przykład:
    /// 
    /// Character (GameObject)
    ///   ├─ Body (Renderer)
    ///   ├─ Head (Renderer)
    ///   │   └─ Hat (Renderer)
    ///   └─ Weapon (Renderer)
    ///       └─ Blade (Renderer)
    /// 
    /// Gdybyśmy chcieli zmienić layer dla CAŁEJ postaci, musielibyśmy:
    /// - character.layer = X
    /// - body.layer = X
    /// - head.layer = X
    /// - hat.layer = X
    /// - weapon.layer = X
    /// - blade.layer = X
    /// 
    /// To jest MĘCZĄCE! Ta funkcja robi to AUTOMATYCZNIE dla wszystkich dzieci.
    /// 
    /// Użycie w tej grze:
    /// -----------------
    /// W menu głównym (LoadoutState) postać jest wyświetlana na specjalnej kamerze UI.
    /// Musimy ustawić jej layer na "UI" żeby tylko kamera UI ją renderowała,
    /// a normalna kamera gry NIE.
    /// 
    /// Parametry:
    /// ----------
    /// root - główny GameObject (np. Character)
    /// layer - numer warstwy do ustawienia (0-31, Unity ma max 32 warstwy)
    /// 
    /// Jak to działa?
    /// -------------
    /// 1. GetComponentsInChildren<Renderer>(true) - znajdź WSZYSTKIE Renderery
    ///    - "true" oznacza że szuka także w nieaktywnych dzieciach
    ///    - Renderer to komponent, który rysuje obiekty (MeshRenderer, SkinnedMeshRenderer, etc.)
    /// 
    /// 2. Dla każdego znalezionego Renderera:
    ///    - Weź jego GameObject (rends[i].gameObject)
    ///    - Ustaw jego layer (layer = X)
    /// 
    /// UWAGA TECHNICZNA:
    /// -----------------
    /// Funkcja ustawia layer na GAMEOBJECT Renderera, nie na samym Rendererze.
    /// GameObject ma pole "layer", Renderer nie ma.
    /// Renderer dziedziczy layer od swojego GameObjecta.
    /// </summary>
    static public void SetRendererLayerRecursive(GameObject root, int layer)
    {
        // Znajdź wszystkie Renderery w tym GameObjekcie i wszystkich jego dzieciach
        // Parametr "true" = szukaj także w nieaktywnych obiektach
        // GetComponentsInChildren ZAWIERA także komponent na "root" (jeśli ma Renderer)
        Renderer[] rends = root.GetComponentsInChildren<Renderer>(true);

        // Przejdź przez wszystkie znalezione Renderery
        for(int i = 0; i < rends.Length; ++i)
        {
            // Ustaw layer na GameObjeccie tego Renderera
            // WAŻNE: Ustawiamy na rends[i].gameObject, NIE na rends[i]!
            // GameObject ma właściwość "layer", Renderer nie ma
            rends[i].gameObject.layer = layer;
        }
    }
}
