# Przewodnik dla Studentów - Endless Runner

## Wprowadzenie

Witaj! Jeśli po raz pierwszy pracujesz z tym projektem, ten przewodnik pomoże Ci zrozumieć jak działa gra Endless Runner i jak jest zorganizowany kod.

## 🎮 Czym jest Endless Runner?

Endless Runner (nieskończony biegacz) to gatunek gier mobilnych, w którym:
- Postać AUTOMATYCZNIE biegnie do przodu
- Gracz kontroluje tylko ruchy w bok, skakanie i zślizgiwanie
- Tor generuje się w nieskończoność (endless = nieskończony)
- Celem jest przebiegnie jak najdłuższy dystans

Przykłady znanych gier tego typu: Temple Run, Subway Surfers

## 📁 Struktura Projektu

```
Assets/
├── Scripts/              # Cały kod gry w C#
│   ├── GameManager/      # Zarządzanie stanami gry (Menu, Gra, GameOver)
│   ├── Characters/       # Postać gracza, sterowanie, kolizje
│   ├── Tracks/           # Generowanie i zarządzanie torem
│   ├── Obstacles/        # Różne rodzaje przeszkód
│   ├── Consumable/       # Power-upy (magnes monet, nieśmiertelność, etc.)
│   ├── UI/               # Interfejs użytkownika
│   ├── Themes/           # Motywy wizualne (różne środowiska)
│   └── Missions/         # System misji/zadań
└── ...
```

## 🏗️ Architektura - Kluczowe Koncepcje

### 1. State Machine (Maszyna Stanów)

Gra używa wzorca "State Machine" do zarządzania różnymi ekranami/trybami.

**Stany w grze:**
- **LoadoutState** - Menu główne (wybór postaci, sklep, statystyki)
- **GameState** - Aktywna rozgrywka (gracz biegnie)
- **GameOverState** - Ekran po przegranej (wynik, opcje)
- **TutorialState** - Tutorial dla nowych graczy

**Jak to działa:**
```
Gracz w menu → LoadoutState
Klika "Start" → PushState("Game") → GameState  
Przegrywa → PushState("GameOver") → GameOverState
Klika "Retry" → PopState() → wraca do GameState
```

**Pliki do przeczytania:**
- `GameManager/GameManager.cs` - Główny manager stanów
- `GameManager/LoadoutState.cs` - Menu główne
- `GameManager/GameState.cs` - Logika rozgrywki
- `GameManager/GameOverState.cs` - Ekran końcowy

### 2. Object Pooling

**Problem:**
Tworzymy SETKI monet i przeszkód podczas gry. Tworzenie (Instantiate) i niszczenie (Destroy) obiektów jest BARDZO WOLNE.

**Rozwiązanie - Pooling:**
1. Na starcie tworzymy 50 monet i ukrywamy je (pula/pool)
2. Gdy potrzebujemy monety - bierzemy z puli i pokazujemy
3. Gdy gracz zbierze monetę - ukrywamy ją i zwracamy do puli
4. Ta sama moneta może być użyta 100 razy!

**Analogia:**
Restauracja nie kupuje nowego talerza dla każdego klienta i nie wyrzuca go po posiłku. Ma zestaw talerzy, które myje i używa ponownie.

**Pliki do przeczytania:**
- `Pooler.cs` - Implementacja systemu poolingu
- `Coin.cs` - Przykład użycia poolera

### 3. Addressables - Dynamiczne Ładowanie

**Co to jest:**
System Unity do ładowania zasobów (postaci, tekstur, dźwięków) w sposób dynamiczny.

**Dlaczego to ważne:**
- Gra nie musi ładować wszystkiego na starcie (szybsze uruchomienie)
- Możesz pobierać zawartość z internetu (DLC, aktualizacje)
- Oszczędzasz pamięć (ładujesz tylko to czego potrzebujesz)

**W tej grze:**
- Postacie są ładowane przez Addressables
- Motywy (themes) są ładowane przez Addressables
- Przeszkody są ładowane przez Addressables

**Jak dodać nową postać:**
1. Stwórz prefab postaci
2. Zaznacz checkbox "Addressable" w Inspectorze
3. Dodaj label "character"
4. Gotowe! Gra automatycznie ją znajdzie

### 4. Shadery i Zakrzywienie Świata

**Problem:**
W endless runner tor ciągnie się w nieskończoność. Gracz widzi "koniec świata" w oddali (brzydkie!).

**Rozwiązanie:**
Zakrzywiamy cały świat w dół (jak Ziemia jest okrągła).

```
Płaski tor:     _____________________  (widać koniec)
Zakrzywiony:    ___________           (koniec ukryty)
                           \___
```

**Jak to działa:**
Shader (program na karcie graficznej) modyfikuje pozycje wierzchołków podczas renderowania. To jest SZYBKIE bo działa na GPU.

**Pliki do przeczytania:**
- `WorldCurver.cs` - Ustawia siłę zakrzywienia

### 5. Coroutines

**Co to jest:**
Funkcja która może "zapauzować" wykonanie i kontynuować później.

**Przykład:**
```csharp
// Normalna funkcja - ZABLOKUJE GRĘ!
void Wait5Seconds() {
    Thread.Sleep(5000); // ŹLE! Wszystko zamarznie!
}

// Coroutine - NIE blokuje
IEnumerator Wait5Seconds() {
    yield return new WaitForSeconds(5); // Gra działa normalnie
}
```

**Kiedy używamy:**
- Ładowanie zasobów w tle (Addressables)
- Animacje rozłożone w czasie
- Opóźnienia (poczekaj X sekund, potem zrób Y)

**Pliki do przeczytania:**
- `CoroutineHandler.cs` - Umożliwia uruchamianie coroutines z klas nie-MonoBehaviour

## 🎯 Najważniejsze Pliki dla Początkujących

### Zacznij od tych plików (są najprostsze):

1. **`Coin.cs`** - Najprostszy skrypt, pokazuje pooling
2. **`Helpers.cs`** - Pomocnicze funkcje (layers)
3. **`WorldCurver.cs`** - Shadery dla początkujących
4. **`OpenURL.cs`** - Otwieranie linków
5. **`LevelLoader.cs`** - Ładowanie scen

### Potem przejdź do:

6. **`Pooler.cs`** - System poolingu obiektów
7. **`CoroutineHandler.cs`** - Obsługa coroutines
8. **`GameManager.cs`** - Maszyna stanów
9. **`Obstacle.cs`** - Klasy abstrakcyjne i dziedziczenie

### Dla zaawansowanych:

10. **`TrackManager.cs`** - Generowanie toru (zaawansowane!)
11. **`CharacterInputController.cs`** - Sterowanie postacią
12. **`PlayerData.cs`** - Zapisywanie danych

## 🔧 Wzorce Projektowe Używane w Grze

### 1. Singleton
**Gdzie:** `GameManager`, `TrackManager`, `PlayerData`

**Co to jest:** Klasa która ma TYLKO JEDNĄ instancję w całej grze.

**Dlaczego:** Niektóre rzeczy powinny istnieć tylko raz (manager gry, dane gracza).

### 2. Object Pooling
**Gdzie:** `Pooler`, `Coin`, przeszkody

**Co to jest:** Ponowne wykorzystanie obiektów zamiast tworzenia nowych.

**Dlaczego:** Tworzenie/niszczenie obiektów jest wolne.

### 3. State Machine (Maszyna Stanów)
**Gdzie:** `GameManager` + wszystkie stany

**Co to jest:** Gra może być w różnych "stanach" (Menu, Gra, GameOver).

**Dlaczego:** Lepsze niż gigantyczne ify sprawdzające "w jakim jesteśmy trybie".

### 4. Strategy Pattern (Wzorzec Strategii)
**Gdzie:** `Modifier` + `LimitedLengthRun`, `SeededRun`, `SingleLifeRun`

**Co to jest:** Różne "strategie" dla różnych trybów gry.

**Dlaczego:** Łatwo dodać nowy tryb bez przepisywania GameState.

### 5. Observer Pattern (częściowo)
**Gdzie:** `newSegmentCreated`, `currentSegementChanged` w TrackManager

**Co to jest:** Powiadamianie innych obiektów o wydarzeniach.

**Dlaczego:** Luźne powiązanie między systemami.

## 📚 Kluczowe Koncepcje Unity

### Layers (Warstwy)
- Każdy GameObject ma przypisaną warstwę (layer)
- Kamery mogą renderować tylko wybrane warstwy
- Używane np. do pokazywania postaci tylko na kamerze UI w menu

### Tags (Tagi)
- Etykiety dla GameObjectów
- Używane do identyfikacji (np. tag "Obstacle", "Coin")
- Szybkie wyszukiwanie: `GameObject.FindGameObjectWithTag("Player")`

### Prefabs
- "Szablony" GameObjectów
- Możesz stworzyć wiele kopii tego samego prefaba
- Zmiana prefaba zmienia wszystkie jego instancje

### Scenes (Sceny)
- Różne "poziomy" lub "ekrany" gry
- Menu główne to jedna scena, gra to druga
- Można ładować sceny addytywnie (jedna na drugiej)

## 🎓 Zadania dla Studenta

### Poziom Początkujący

1. **Zmień siłę zakrzywienia świata**
   - Otwórz `WorldCurver.cs`
   - Znajdź GameObject z tym skryptem w scenie
   - Zmień wartość `curveStrength` w Inspectorze
   - Zobacz jak zmienia się wygląd toru!

2. **Dodaj więcej monet**
   - Znajdź gdzie tworzy się pool monet
   - Zwiększ początkową wielkość puli
   - Zobacz różnicę w wydajności

3. **Zmień prędkość gry**
   - Otwórz `TrackManager.cs`
   - Znajdź `minSpeed` i `maxSpeed`
   - Eksperymentuj z wartościami

### Poziom Średni

4. **Stwórz nową przeszkodę**
   - Utwórz nową klasę dziedziczącą po `Obstacle`
   - Zaimplementuj funkcję `Spawn()`
   - Stwórz prefab i oznacz jako Addressable

5. **Dodaj nowy power-up**
   - Utwórz klasę dziedziczącą po `Consumable`
   - Zaimplementuj efekt power-upu
   - Dodaj ikonę i dźwięk

### Poziom Zaawansowany

6. **Dodaj nowy tryb gry**
   - Stwórz klasę dziedziczącą po `Modifier`
   - Zaimplementuj własne zasady
   - np. "Zbierz 100 monet w 60 sekund"

7. **Stwórz nowy motyw**
   - Zaprojektuj nowe modele 3D dla toru
   - Stwórz ThemeData
   - Oznacz jako Addressable z label "theme"

## 🐛 Debugowanie - Przydatne Wskazówki

### Problem: Gra się zacina
- **Sprawdź:** Czy używasz Poolingu? Czy nie tworzysz zbyt wielu obiektów?
- **Debug:** Otwórz Profiler (Window > Analysis > Profiler)

### Problem: Obiekt nie ładuje się (Addressables)
- **Sprawdź:** Czy prefab jest oznaczony jako Addressable?
- **Sprawdź:** Czy zbudowałeś Addressables? (Window > Asset Management > Addressables > Groups > Build > New Build)
- **Debug:** Szukaj błędów w Console

### Problem: Kolizje nie działają
- **Sprawdź:** Czy obiekty mają Collidery?
- **Sprawdź:** Czy są na odpowiednich Layerach?
- **Debug:** Włącz Gizmos w Scene view (przycisk na górze)

### Problem: Postać nie reaguje na input
- **Sprawdź:** Czy CharacterInputController jest włączony?
- **Sprawdź:** Czy GameState jest aktywny?
- **Debug:** Dodaj `Debug.Log()` w funkcjach inputu

## 📖 Dodatkowe Zasoby

### Dokumentacja Unity
- [Unity Manual](https://docs.unity3d.com/Manual/index.html)
- [Unity Scripting API](https://docs.unity3d.com/ScriptReference/)
- [Unity Learn](https://learn.unity.com/)

### Polecane Tutorials
- [Tworzenie gry mobilnej](https://learn.unity.com/course/create-with-code)
- [Podstawy C#](https://learn.unity.com/course/programming-for-unity)
- [Object Pooling](https://learn.unity.com/tutorial/object-pooling)

## ❓ Najczęściej Zadawane Pytania

**P: Dlaczego używamy `k_` przed stałymi?**  
O: To konwencja Unity/Google Style Guide. `k_` oznacza constant (stała).

**P: Co oznacza `m_` przed zmiennymi?**  
O: `m_` oznacza member variable (zmienna członkowska klasy).

**P: Czym się różni `public` od `protected` od `private`?**  
O:
- `public` - dostępne wszędzie
- `protected` - dostępne w tej klasie i klasach pochodnych
- `private` - dostępne tylko w tej klasie

**P: Kiedy używać `static`?**  
O: Gdy funkcja/zmienna należy do KLASY, nie do instancji obiektu.

**P: Co to jest `yield return`?**  
O: To pauza w Coroutine. Funkcja zatrzymuje się i wznawia później.

## 🎉 Powodzenia!

Pamiętaj:
- Eksperymentuj! Najlepszy sposób nauki to próbowanie
- Czytaj komentarze w kodzie - są tam szczegółowe wyjaśnienia
- Nie bój się błędów - każdy programista je popełnia
- Używaj `Debug.Log()` żeby zrozumieć co się dzieje
- Zadawaj pytania!

Miłej nauki! 🚀
