using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Pooler - System poolingu obiektów (ponownego wykorzystania)
/// 
/// DLA STUDENTÓW - WAŻNA KONCEPCJA W TWORZENIU GIER:
/// ==================================================
/// 
/// Problem:
/// --------
/// W grach często musimy tworzyć i niszczyć WIELE obiektów (monety, przeszkody, kule itp.)
/// Przykład: W endless runner możemy mieć SETKI monet podczas jednej rozgrywki.
/// 
/// Naiwne podejście (ZŁE):
/// -----------------------
/// - Gdy potrzebujemy monety: Instantiate(coinPrefab) - tworzymy nowy obiekt
/// - Gdy gracz zbierze monetę: Destroy(coin) - niszczymy obiekt
/// 
/// Dlaczego to jest ZŁE?
/// - Instantiate() jest WOLNE (alokacja pamięci, inicjalizacja komponentów)
/// - Destroy() jest WOLNE (sprzątanie pamięci, garbage collection)
/// - Gdy robimy to setki razy, gra się ZACINA (spadki FPS)
/// 
/// Rozwiązanie - Object Pooling:
/// ------------------------------
/// Zamiast tworzyć i niszczyć, tworzymy "pulę" obiektów na początku i używamy wielokrotnie.
/// 
/// Analogia ze świata rzeczywistego:
/// Wyobraź sobie restaurację:
/// - BAD: Przy każdym kliencie kupuj nowy talerz, po posiłku go wyrzuć (Instantiate/Destroy)
/// - GOOD: Miej zestaw talerzy, przy kliencie daj czysty talerz, po posiłku umyj i odłóż (Pooling)
/// 
/// Jak działa ten Pooler:
/// -----------------------
/// 1. Konstruktor tworzy N obiektów na start (np. 50 monet)
/// 2. Wszystkie są nieaktywne i czekają na stosie (m_FreeInstances)
/// 3. Get() - pobiera obiekt ze stosu, aktywuje go i zwraca
/// 4. Free() - dezaktywuje obiekt i zwraca na stos
/// 5. Ten sam obiekt może być użyty 1000 razy!
/// 
/// Stack (Stos):
/// -------------
/// Stack to struktura danych "LIFO" (Last In, First Out - ostatni wszedł, pierwszy wyszedł)
/// Jak stos talerzy:
/// - Push() - kładzie talerz NA WIERZCH stosu
/// - Pop() - zdejmuje talerz Z WIERZCHU stosu
/// - Peek() - patrzy na talerz Z WIERZCHU (bez zdejmowania)
/// 
/// Dlaczego Stack a nie List?
/// - Stack.Pop() jest O(1) - bardzo szybkie
/// - Nie obchodzi nas KTÓRY obiekt dostaniemy, byle jakiś wolny
/// </summary>
public class Pooler
{
    // Stack przechowujący wolne (nieużywane) instancje obiektów
    // Gdy obiekt jest zwolniony (Free), ląduje tutaj
    // Gdy potrzebujemy obiektu (Get), bierzemy go stąd
	protected Stack<GameObject> m_FreeInstances = new Stack<GameObject>();
	
    // Oryginalny prefab - używany gdy Stack jest pusty i musimy stworzyć nową instancję
    protected GameObject m_Original;

    /// <summary>
    /// Konstruktor Poolera
    /// 
    /// Parametry:
    /// - original: Prefab który będziemy klonować
    /// - initialSize: Ile obiektów stworzyć na start
    /// 
    /// Proces:
    /// 1. Zapisujemy referencję do oryginału
    /// 2. Tworzymy Stack o odpowiedniej pojemności (optymalizacja)
    /// 3. Tworzymy initialSize obiektów:
    ///    - Instantiate (klonujemy prefab)
    ///    - SetActive(false) (ukrywamy - nie są jeszcze używane)
    ///    - Push do Stacka (dodajemy do puli wolnych)
    /// </summary>
	public Pooler(GameObject original, int initialSize)
	{
		m_Original = original;
        // Tworzymy Stack z początkową pojemnością - unika realokacji pamięci później
		m_FreeInstances = new Stack<GameObject>(initialSize);

        // Tworzymy początkową pulę obiektów
		for (int i = 0; i < initialSize; ++i)
		{
            // Klonujemy prefab
			GameObject obj = Object.Instantiate(original);
            // Dezaktywujemy - obiekt istnieje ale jest "niewidzialny" i nie działa
			obj.SetActive(false);
            // Dodajemy do puli wolnych obiektów
            m_FreeInstances.Push(obj);
		}
	}

    /// <summary>
    /// Get - pobiera obiekt z puli (wersja uproszczona)
    /// 
    /// Zwraca obiekt na pozycji (0,0,0) bez rotacji
    /// Wewnętrznie wywołuje Get(Vector3.zero, Quaternion.identity)
    /// </summary>
	public GameObject Get()
	{
		return Get(Vector3.zero, Quaternion.identity);
	}

    /// <summary>
    /// Get - pobiera obiekt z puli na określonej pozycji i rotacji
    /// 
    /// To jest GŁÓWNA funkcja poolera!
    /// 
    /// Algorytm:
    /// 1. Jeśli Stack ma wolne obiekty (Count > 0):
    ///    - Pop() - zdejmij obiekt ze stosu
    /// 2. Jeśli Stack jest pusty:
    ///    - Instantiate() - stwórz NOWY obiekt (pula się powiększa dynamicznie)
    /// 3. Ustaw obiekt jako aktywny (SetActive(true))
    /// 4. Ustaw pozycję i rotację
    /// 5. Zwróć obiekt
    /// 
    /// UWAGA: 
    /// Jeśli initialSize był zbyt mały, pool automatycznie się rozszerzy.
    /// To bezpieczne, ale lepiej ustawić initialSize wystarczająco duży na starcie.
    /// </summary>
	public GameObject Get(Vector3 pos, Quaternion quat)
	{
        // Operator trójargumentowy (ternary): warunek ? jeśli_tak : jeśli_nie
        // To skrócony if-else w jednej linii
        // Jeśli mamy wolne obiekty, użyj Pop(), w przeciwnym razie stwórz nowy
	    GameObject ret = m_FreeInstances.Count > 0 ? m_FreeInstances.Pop() : Object.Instantiate(m_Original);

        // Aktywuj obiekt (teraz będzie widoczny i działający)
		ret.SetActive(true);
        // Ustaw pozycję w świecie
		ret.transform.position = pos;
        // Ustaw rotację
		ret.transform.rotation = quat;

		return ret;
	}

    /// <summary>
    /// Free - zwraca obiekt do puli (zwalnia go)
    /// 
    /// Wywołaj tę funkcję gdy skończyłeś używać obiekt i chcesz go "oddać" do puli.
    /// Obiekt NIE jest niszczony - jest tylko ukrywany i czeka na ponowne użycie.
    /// 
    /// Proces:
    /// 1. SetParent(null) - odłącza obiekt od rodzica w hierarchii
    ///    (gdyby był dzieckiem czegoś, np. Track Segment)
    /// 2. SetActive(false) - dezaktywuje obiekt (nie renderowany, brak fizyki, brak Update)
    /// 3. Push() - wrzuca obiekt z powrotem na stos wolnych instancji
    /// 
    /// WAŻNE:
    /// Po wywołaniu Free(), obiekt dalej ISTNIEJE w pamięci, po prostu jest nieaktywny.
    /// Następne wywołanie Get() może zwrócić DOKŁADNIE TEN SAM obiekt!
    /// </summary>
	public void Free(GameObject obj)
	{
        // Odłącz od rodzica - obiekt nie powinien być dzieckiem niczego w puli
		obj.transform.SetParent(null);
        // Dezaktywuj - oszczędność CPU/GPU (brak renderowania, fizyki, skryptów)
		obj.SetActive(false);
        // Wrzuć z powrotem na stos wolnych obiektów
		m_FreeInstances.Push(obj);
	}
}
