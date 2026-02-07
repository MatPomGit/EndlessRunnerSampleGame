using UnityEngine;
using System;

/// <summary>
/// CoinMagnet - Power-up magnesu na monety
/// 
/// DLA STUDENTÓW - FIZYKA W UNITY:
/// ================================
/// 
/// Co robi ten power-up:
/// --------------------
/// - Przyciąga monety z dużej odległości
/// - Gracz NIE musi celować - monety same "lecą" do niego
/// - Łatwiej zbierać wszystkie monety na torze
/// 
/// Jak to działa technicznie:
/// -------------------------
/// Co klatkę (Tick):
/// 1. Sprawdzamy obszar wokół gracza (Physics.OverlapBox)
/// 2. Znajdujemy wszystkie monety w tym obszarze
/// 3. "Przyczepiamy" je do gracza (SetParent)
/// 4. Monety podążają za graczem dopóki ich nie zbierze
/// 
/// Physics.OverlapBox - WAŻNA KONCEPCJA:
/// -------------------------------------
/// To funkcja Unity która znajduje wszystkie collidery w pudełku (box).
/// 
/// Wyobraź sobie niewidzialny prostokąt wokół gracza:
/// 
///        [ ................ ]  <- k_HalfExtentsBox (szerokość: 40m!)
///             (gracz)
/// 
/// Wszystkie monety w tym prostokącie są "schwytane" przez magnes.
/// 
/// k_HalfExtentsBox = (20, 1, 1):
/// ------------------------------
/// - X: 20 metrów (łącznie 40m szerokości - bardzo szeroko!)
/// - Y: 1 metr (w górę i w dół)
/// - Z: 1 metr (do przodu i tyłu)
/// 
/// To duży obszar - monety są przyciągane z daleka!
/// 
/// k_LayerMask = 1 << 8:
/// ---------------------
/// Layer Mask określa KTÓRE warstwy (layers) sprawdzać.
/// 1 << 8 to operacja bitowa (bit shifting):
/// - Przesuwa bit "1" o 8 pozycji w lewo
/// - Wynik: 256 (binarne: 100000000)
/// - Oznacza: "sprawdzaj tylko layer numer 8"
/// 
/// W projekcie layer 8 = "Collectable" (monety, power-upy)
/// Dzięki temu NIE sprawdzamy przeszkód, toru, tylko MONETY.
/// 
/// returnColls - optymalizacja:
/// ----------------------------
/// Tworzymy tablicę RAZ (jako pole klasy), używamy wielokrotnie.
/// Gdybyśmy tworzyli nową tablicę co klatkę = garbage collection = lagi!
/// 
/// NonAlloc:
/// --------
/// Physics.OverlapBoxNonAlloc NIE alokuje nowej pamięci.
/// Wypełnia istniejącą tablicę (returnColls).
/// To optymalizacja - brak garbage collection.
/// 
/// Cena:
/// -----
/// 750 monet (tańsze niż Invincibility)
/// 0 premium (nie można kupić za klejnoty)
/// </summary>
public class CoinMagnet : Consumable
{
    // Rozmiar obszaru w którym magnes działa
    // (20, 1, 1) = 40m szerokość, 2m wysokość, 2m głębokość
    // readonly = stała, nie może być zmieniona po utworzeniu
    protected readonly Vector3 k_HalfExtentsBox = new Vector3 (20.0f, 1.0f, 1.0f);
    
    // Layer Mask - tylko layer 8 (Collectable)
    // 1 << 8 = bit shifting, wynik: 256
    // Sprawdzamy TYLKO monety, ignorujemy przeszkody/tor
    protected const int k_LayerMask = 1 << 8;

    /// <summary>
    /// Nazwa power-upu w UI
    /// </summary>
    public override string GetConsumableName()
    {
        return "Magnet";
    }

    /// <summary>
    /// Typ power-upu (enum do identyfikacji w kodzie)
    /// </summary>
    public override ConsumableType GetConsumableType()
    {
        return ConsumableType.COIN_MAG;
    }

    /// <summary>
    /// Cena w zwykłych monetach
    /// 750 = tańsze niż Invincibility (1500)
    /// </summary>
    public override int GetPrice()
    {
        return 750;
    }

    /// <summary>
    /// Cena w premium walucie
    /// 0 = NIE można kupić za klejnoty (tylko za zwykłe monety)
    /// </summary>
	public override int GetPremiumCost()
	{
		return 0;
	}

    // Tablica do przechowywania znalezionych colliderów
    // OPTYMALIZACJA: Tworzymy RAZ, używamy wielokrotnie
    // Maksymalnie 20 monet naraz w zasięgu magnesu
	protected Collider[] returnColls = new Collider[20];

    /// <summary>
    /// Tick - wywoływane CO KLATKĘ gdy magnes jest aktywny
    /// 
    /// Algorytm:
    /// --------
    /// 1. Physics.OverlapBoxNonAlloc - znajdź wszystkie collidery w pudełku
    /// 2. Dla każdego znalezionego collidera:
    ///    a) Sprawdź czy to moneta (GetComponent<Coin>())
    ///    b) Sprawdź czy to NIE premium moneta (isPremium)
    ///    c) Sprawdź czy jeszcze NIE jest przyciągnięta
    ///    d) Jeśli wszystko OK: przyczepi do gracza (SetParent)
    /// 
    /// Dlaczego premium monety są ignorowane?
    /// --------------------------------------
    /// Premium monety (klejnoty) NIE powinny być magnesowane.
    /// Gracz musi celowo po nie pobiec - to zwiększa challenge.
    /// 
    /// magnetCoins lista:
    /// -----------------
    /// Lista monet już przyciągniętych.
    /// Zapobiega przyciąganiu tej samej monety wielokrotnie.
    /// </summary>
	public override void Tick(CharacterInputController c)
    {
        base.Tick(c); // Aktualizuj timer power-upu

        // Znajdź wszystkie collidery w pudełku wokół gracza
        // OverlapBoxNonAlloc = NIE tworzy nowej tablicy (optymalizacja!)
        // Wypełnia istniejącą tablicę returnColls
        // Zwraca LICZBĘ znalezionych colliderów (nb = number)
        int nb = Physics.OverlapBoxNonAlloc(
            c.characterCollider.transform.position,  // Centrum pudełka (pozycja gracza)
            k_HalfExtentsBox,                        // Rozmiar pudełka (20, 1, 1)
            returnColls,                             // Tablica do wypełnienia
            c.characterCollider.transform.rotation,  // Rotacja pudełka (jak gracz)
            k_LayerMask                              // Tylko layer 8 (Collectable)
        );

        // Przejdź przez wszystkie znalezione collidery
        for(int i = 0; i< nb; ++i)
        {
            // Pobierz komponent Coin z collidera
            // Jeśli collider NIE ma Coin komponentu, zwróci null
			Coin returnCoin = returnColls[i].GetComponent<Coin>();

            // Sprawdź czy:
            // 1. To jest moneta (returnCoin != null)
            // 2. To NIE jest premium moneta (!isPremium)
            // 3. Ta moneta NIE jest jeszcze w liście przyciągniętych (!Contains)
			if (returnCoin != null && !returnCoin.isPremium && !c.characterCollider.magnetCoins.Contains(returnCoin.gameObject))
			{
                // Przyczepi monetę do gracza jako dziecko (child)
                // Teraz moneta będzie podążać za graczem!
				returnColls[i].transform.SetParent(c.transform);
                
                // Dodaj do listy przyciągniętych monet
                // Zapobiega przyciąganiu tej samej monety dwa razy
				c.characterCollider.magnetCoins.Add(returnColls[i].gameObject);
			}
		}
    }
}
