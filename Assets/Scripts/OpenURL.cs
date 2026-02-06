using UnityEngine;

/// <summary>
/// OpenURL - Prosty skrypt do otwierania stron internetowych
/// 
/// DLA STUDENTÓW:
/// ==============
/// Ten skrypt pokazuje jak otworzyć przeglądarkę z gry Unity.
/// 
/// Użycie:
/// -------
/// 1. Dodaj ten skrypt do przycisku UI
/// 2. Ustaw 'websiteAddress' w Inspectorze (np. "https://unity.com")
/// 3. Podłącz funkcję OpenURLOnClick() do przycisku (OnClick event)
/// 
/// Przykład w tej grze:
/// -------------------
/// Używany w menu do otwarcia strony Unity Learn, social media itp.
/// 
/// Application.OpenURL():
/// ---------------------
/// To funkcja Unity która otwiera URL w domyślnej przeglądarce systemu.
/// - Na PC/Mac: otwiera Chrome/Safari/Firefox
/// - Na Android: otwiera Chrome/przeglądarkę systemu
/// - Na iOS: otwiera Safari
/// 
/// UWAGA dla studentów:
/// -------------------
/// Na mobilnych platformach (Android/iOS) aplikacja może przejść w tło
/// gdy przeglądarka się otworzy. Pamiętaj o obsłudze OnApplicationPause()
/// jeśli twoja gra wymaga specjalnej logiki pauzowania!
/// </summary>
public class OpenURL : MonoBehaviour
{
    // Adres strony internetowej do otwarcia
    // Ustawiane w Inspectorze Unity (np. "https://unity.com")
    // MUSI zaczynać się od "http://" lub "https://"
    public string websiteAddress;

    /// <summary>
    /// OpenURLOnClick - Otwiera stronę internetową w przeglądarce
    /// 
    /// Ta funkcja jest wywoływana przez przycisk UI (OnClick event).
    /// 
    /// Jak podłączyć do przycisku:
    /// ---------------------------
    /// 1. Zaznacz przycisk UI w scenie
    /// 2. W Inspectorze znajdź komponent Button
    /// 3. W sekcji "On Click ()" kliknij "+"
    /// 4. Przeciągnij GameObject z tym skryptem do pola
    /// 5. Z menu wybierz OpenURL -> OpenURLOnClick()
    /// 6. Gotowe! Kliknięcie przycisku otworzy stronę
    /// 
    /// Application.OpenURL() działa na wszystkich platformach:
    /// - Windows/Mac/Linux - otworzy domyślną przeglądarkę
    /// - Android/iOS - otworzy przeglądarkę mobilną
    /// - WebGL - otworzy nową kartę przeglądarki
    /// </summary>
    public void OpenURLOnClick()
    {
        // Otwórz adres URL w domyślnej przeglądarce systemu
        Application.OpenURL(websiteAddress);
    }
}
