using UnityEngine;

/// <summary>
/// Coin - Klasa reprezentująca monetę do zbierania w grze
/// 
/// DLA STUDENTÓW:
/// To jest bardzo prosty skrypt, ale pokazuje ważną koncepcję - Object Pooling.
/// 
/// Co to jest Pooling?
/// -------------------
/// Zamiast tworzyć i niszczyć monety ciągle (co jest WOLNE), tworzymy pulę (pool)
/// monet na początku i używamy ich wielokrotnie.
/// 
/// Jak to działa?
/// 1. Na starcie gry tworzymy np. 50 monet i ukrywamy je
/// 2. Gdy potrzebujemy monety, bierzemy jedną z puli i pokazujemy
/// 3. Gdy gracz zbierze monetę, ukrywamy ją i zwracamy do puli
/// 4. Ta sama moneta może być użyta 100 razy podczas jednej rozgrywki!
/// 
/// Dlaczego to jest ważne?
/// - Instantiate (tworzenie obiektów) jest WOLNE
/// - Destroy (niszczenie obiektów) jest WOLNE  
/// - Pooling jest SZYBKI
/// - W grze typu endless runner tworzymy SETKI monet - bez poolingu gra by się zacinała
/// 
/// Pola w tej klasie:
/// ------------------
/// coinPool - statyczny Pooler, który zarządza wszystkimi monetami w grze
/// isPremium - czy to jest zwykła moneta czy premium (diamond/klejnot)?
/// </summary>
public class Coin : MonoBehaviour
{
    // Statyczny pooler - JEDEN pooler dla WSZYSTKICH monet
    // "static" oznacza że ta zmienna jest współdzielona między wszystkimi monetami
    // Nie każda moneta ma swój pooler - WSZYSTKIE monety używają tego samego
	static public Pooler coinPool;
    
    // Czy to jest premium coin (diamond/klejnot)?
    // false = zwykła moneta (żółta)
    // true = premium moneta (niebieski klejnot, wartość więcej)
    public bool isPremium = false;
}
