using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Invincibility - Power-up nieśmiertelności
/// 
/// DLA STUDENTÓW:
/// ==============
/// To power-up (przedmiot zużywalny) który sprawia że gracz jest niezniszczalny.
/// 
/// Co robi:
/// --------
/// - Przez określony czas (duration) gracz NIE może przegrać
/// - Uderzenie w przeszkodę nie zabiera życia
/// - Postać świeci/migocze pokazując że jest nieśmiertelna
/// - Po czasie efekt znika
/// 
/// Jak działa technicznie:
/// ----------------------
/// 1. Started() - włącza nieśmiertelność na czas 'duration'
/// 2. Tick() - co klatkę wymusza nieśmiertelność (na wypadek gdyby coś próbowało ją wyłączyć)
/// 3. Ended() - wyłącza nieśmiertelność po upływie czasu
/// 
/// Dziedziczenie:
/// -------------
/// Ta klasa dziedziczy po Consumable.cs (klasa bazowa wszystkich power-upów).
/// Musi zaimplementować:
/// - GetConsumableName() - nazwa power-upu
/// - GetConsumableType() - typ (enum)
/// - GetPrice() - cena w monetach
/// - GetPremiumCost() - cena w premium walucie
/// 
/// Cena:
/// -----
/// 1500 monet ALBO 5 premium (diamonds)
/// Drogie, bo bardzo potężne!
/// </summary>
public class Invincibility : Consumable
{
    /// <summary>
    /// Zwraca nazwę power-upu wyświetlaną w UI
    /// </summary>
    public override string GetConsumableName()
    {
        return "Invincible"; // Nazwa w języku angielskim (UI)
    }

    /// <summary>
    /// Zwraca typ power-upu (używany do identyfikacji w kodzie)
    /// </summary>
    public override ConsumableType GetConsumableType()
    {
        return ConsumableType.INVINCIBILITY;
    }

    /// <summary>
    /// Zwraca cenę w zwykłych monetach (żółte monety)
    /// 1500 to dużo - power-up jest bardzo mocny!
    /// </summary>
    public override int GetPrice()
    {
        return 1500;
    }

    /// <summary>
    /// Zwraca cenę w premium walucie (niebieskie klejnoty/diamonds)
    /// 5 premium = około 1500 zwykłych monet
    /// Gracze mogą kupić za prawdziwe pieniądze
    /// </summary>
	public override int GetPremiumCost()
	{
		return 5;
	}

    /// <summary>
    /// Tick - wywoływane CO KLATKĘ gdy power-up jest aktywny
    /// 
    /// SetInvincibleExplicit(true):
    /// ---------------------------
    /// "Explicit" oznacza "wyraźne, jawne".
    /// Wymuszamy nieśmiertelność KAŻDĄ klatkę.
    /// 
    /// Dlaczego co klatkę?
    /// ------------------
    /// Gdyby jakiś inny system próbował wyłączyć nieśmiertelność
    /// (np. przez błąd), ta funkcja natychmiast ją przywróci.
    /// 
    /// To gwarantuje że podczas trwania power-upu gracz
    /// ZAWSZE jest nieśmiertelny, bez wyjątków.
    /// 
    /// base.Tick(c) - wywołuje Tick() z klasy bazowej (Consumable)
    /// Tam aktualizuje się timer power-upu.
    /// </summary>
	public override void Tick(CharacterInputController c)
    {
        base.Tick(c); // Aktualizuj timer (czas trwania)

        // Wymuś nieśmiertelność (gwarantuje że nic jej nie wyłączy)
        c.characterCollider.SetInvincibleExplicit(true);
    }

    /// <summary>
    /// Started - wywoływane gdy power-up się WŁĄCZA
    /// 
    /// IEnumerator = Coroutine:
    /// -----------------------
    /// Może czekać na zakończenie ładowania efektów (partikle, dźwięki).
    /// 
    /// yield return base.Started(c):
    /// ----------------------------
    /// Czeka na zakończenie Started() z klasy bazowej.
    /// Klasa bazowa ładuje:
    /// - Efekt cząsteczkowy (particle system)
    /// - Dźwięk aktywacji
    /// 
    /// SetInvincible(duration):
    /// -----------------------
    /// Włącza nieśmiertelność na określony czas.
    /// 'duration' to pole z klasy bazowej (np. 10 sekund).
    /// 
    /// Różnica SetInvincible vs SetInvincibleExplicit:
    /// -----------------------------------------------
    /// SetInvincible(duration) - ustawia timer na X sekund
    /// SetInvincibleExplicit(true) - wymusza stan na true TERAZ
    /// </summary>
    public override IEnumerator Started(CharacterInputController c)
    {
        // Poczekaj na zakończenie Started() z klasy bazowej
        // (ładowanie dźwięku, efektów cząsteczkowych)
        yield return base.Started(c);
        
        // Włącz nieśmiertelność na czas 'duration'
        // CharacterCollider będzie automatycznie odliczać czas
        c.characterCollider.SetInvincible(duration);
    }

    /// <summary>
    /// Ended - wywoływane gdy power-up się KOŃCZY
    /// 
    /// SetInvincibleExplicit(false):
    /// ----------------------------
    /// Wyłączamy nieśmiertelność WYRAŹNIE.
    /// Gracz znów może przegrać przy uderzeniu w przeszkodę.
    /// 
    /// base.Ended(c):
    /// -------------
    /// Wywołuje Ended() z klasy bazowej.
    /// Tam czyszczone są efekty cząsteczkowe, dźwięki itp.
    /// </summary>
    public override void Ended(CharacterInputController c)
    {
        base.Ended(c); // Wyczyść efekty, dźwięki (klasa bazowa)
        
        // Wyłącz nieśmiertelność - gracz znów jest "śmiertelny"
        c.characterCollider.SetInvincibleExplicit(false);
    }
}
