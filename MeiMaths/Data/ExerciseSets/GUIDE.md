# Guide: Sådan laver du opgavesæt (JSON)

Alle opgavesæt ligger i `Data/ExerciseSets/` som `.json` filer.
Tilføj en ny fil → den dukker automatisk op på forsiden, grupperet efter kategori.

---

## Grundstruktur

```json
{
  "id": "unikt-id",
  "category": "Tal og algebra",
  "title": "Titel",
  "description": "Beskrivelse",
  "questions": [ ... ]
}
```

### Kategorier

| Kategori                       | Icon | Eksempel-ID                      |
|--------------------------------|------|----------------------------------|
| `"Tal og algebra"`             | 🔢   | `"broeker-og-procent"`           |
| `"Geometri"`                   | 📐   | `"geometri-areal"`               |
| `"Statistik og sandsynlighed"` | 📊   | `"statistik-sandsynlighed"`      |
| `"Funktioner"`                 | 📈   | `"lineaere-funktioner"`          |
| `"Andet"`                      | 📘   | (standard hvis ingen kategori)   |

Du kan frit opfinde nye kategorier. Forsiden grupperer automatisk.

### ID-konvention

Brug ID til at angive emne: `"tal-algebra-potenser"`, `"geometri-rumfang"`, `"funktioner-hældning"`, osv.

---

## Spørgsmålstyper

**Alle typer kan blandes frit i samme opgavesæt.** En opgave kan gå fra info → multiple choice → fritekst → info → fritekst osv.

### Multiple choice

```json
{
  "id": 1,
  "type": "multiple-choice",
  "text": "Hvad er 2 + 3?",
  "options": ["3", "4", "5", "6"],
  "correctAnswer": "5",
  "hint": "Tæl videre fra 2"
}
```

### Fritekst

```json
{
  "id": 2,
  "type": "free-text",
  "text": "Hvad er 7 × 8?",
  "correctAnswer": "56",
  "hint": "7 × 8 = 7 × 10 − 7 × 2"
}
```

> **Numerisk sammenligning:** Svar med decimaler sammenlignes med tolerance ±0.01.
> Brugeren kan skrive komma eller punktum (fx `5,83` eller `5.83`).

### Information (ingen svar)

Brug `"type": "info"` til at vise forklarende tekst mellem opgaver.
Info-slides tæller **ikke** i scoren.

```json
{
  "id": 3,
  "type": "info",
  "text": "Hvad er en ligning?",
  "content": "En ligning er et udtryk hvor to sider er lige store.<br><br>Eksempel: <strong>2x + 3 = 7</strong>",
  "correctAnswer": "",
  "hint": ""
}
```

- `"text"` = overskrift (vises med 📖 ikon)
- `"content"` = brødtekst (understøtter HTML: `<br>`, `<strong>`, `<code>`, osv.)
- `"correctAnswer"` og `"hint"` skal være tomme strenge `""`
- Info-slides vises med en blå kant og ingen svarfelter

---

## Graf / koordinatsystem

Tilføj `"graph"` til et spørgsmål for at vise et koordinatsystem med funktioner.

```json
{
  "id": 1,
  "type": "multiple-choice",
  "text": "Hvad er hældningen?",
  "graph": {
    "functions": [
      { "expression": "2x+1", "color": "#0d6efd", "label": "f(x) = 2x + 1" }
    ],
    "xMin": -5, "xMax": 5,
    "yMin": -5, "yMax": 12
  },
  "options": ["1", "2", "3"],
  "correctAnswer": "2",
  "hint": "Hældningen er tallet foran x"
}
```

### Understøttede udtryk

| Udtryk             | Eksempel             | Resultat                    |
|--------------------|----------------------|-----------------------------|
| Lineær             | `"2x+3"`             | f(x) = 2x + 3              |
| Parabel            | `"x^2-4"`            | f(x) = x² − 4              |
| Kubisk             | `"x^3"`              | f(x) = x³                  |
| Eksponentiel       | `"exp(0.5x)"`        | f(x) = e^(0.5x)            |
| Kvadratrod         | `"sqrt(x)"`          | f(x) = √x                  |
| Sinus              | `"sin(x)"`           | f(x) = sin(x)              |
| Cosinus            | `"cos(x)"`           | f(x) = cos(x)              |
| Tangens            | `"tan(x)"`           | f(x) = tan(x)              |
| Logaritme          | `"ln(x)"` / `"log(x)"` | f(x) = ln(x)            |
| Absolut værdi      | `"abs(x)"`           | f(x) = |x|                 |
| Pi                 | `"sin(pi*x)"`        | f(x) = sin(πx)             |
| Sammensat          | `"2sin(x)+x^2"`      | f(x) = 2sin(x) + x²       |

### Flere funktioner i samme graf

```json
"functions": [
  { "expression": "x+2", "color": "#0d6efd", "label": "f(x) = x + 2" },
  { "expression": "x-1", "color": "#dc3545", "label": "g(x) = x - 1" }
]
```

---

## Trekant (trigonometri)

Tilføj `"triangle"` i `"graph"` for at tegne en trekant med labels.

### Konventioner

- **side a** = modsat vinkel A (side BC)
- **side b** = modsat vinkel B (side AC)
- **side c** = modsat vinkel C (side AB)
- Vinkler angives i **grader**

### Gyldige konfigurationer

Du skal angive **mindst 3 værdier** der entydigt bestemmer trekanten:

| Type | Du angiver                      | Eksempel                                      |
|------|---------------------------------|-----------------------------------------------|
| SSS  | 3 sider                        | `sideA: 3, sideB: 4, sideC: 5`               |
| SAS  | 2 sider + indeholdt vinkel      | `sideA: 3, sideB: 4, angleC: 90`             |
| AAS  | 2 vinkler + 1 side              | `angleA: 30, angleC: 90, sideC: 10`          |
| ASA  | 2 vinkler + indeholdt side      | `angleA: 45, angleB: 60, sideC: 8`           |
| SSA  | 2 sider + modsat vinkel         | `sideA: 5, sideC: 13, angleC: 90`            |

> **Vigtigt:** Alle 5 typer virker. Solveren udfylder de manglende sider/vinkler automatisk.

### Eksempel: retvinklet trekant (Pythagoras)

```json
{
  "id": 1,
  "type": "free-text",
  "text": "Find hypotenusen c.",
  "graph": {
    "triangle": {
      "sideA": 3,
      "sideB": 4,
      "angleC": 90,
      "labels": {
        "sideA": "a = 3",
        "sideB": "b = 4",
        "sideC": "c = ?",
        "angleC": "90°"
      }
    }
  },
  "correctAnswer": "5",
  "hint": "c² = a² + b²"
}
```

### Eksempel: find vinkel (SSA)

```json
{
  "id": 2,
  "type": "multiple-choice",
  "text": "Hvad er vinkel A?",
  "graph": {
    "triangle": {
      "sideA": 6,
      "sideC": 12,
      "angleC": 90,
      "labels": {
        "sideA": "a = 6",
        "sideC": "c = 12",
        "angleA": "A = ?",
        "angleC": "90°"
      }
    }
  },
  "options": ["30°", "45°", "60°"],
  "correctAnswer": "30°",
  "hint": "sin(A) = a/c = 0,5"
}
```

### Labels

- Labels er **valgfrie** — kun de labels du angiver vises.
- Brug `"sideA"`, `"sideB"`, `"sideC"` for sidelabels.
- Brug `"angleA"`, `"angleB"`, `"angleC"` for vinkellabels.
- En retvinkel (90°) vises automatisk med en lille firkant (□).

---

## Tips

- `"id"` skal være unikt **inden for** hvert opgavesæt.
- `"correctAnswer"` er altid en **streng** (tal skrives som `"5"`, ikke `5`).
- Brug Unicode-escape for specialtegn: `\u00d7` = ×, `\u00f7` = ÷, `\u2212` = −, `\u00b0` = °.
- Eller skriv dem direkte: `"Hvad er 3 × 4?"` virker også.

---

## Emne-eksempler

Alle emner bruger **samme JSON-struktur**. Her er ét eksempel for hvert emne:

### 🔢 Brøker, decimaltal, procent

```json
{ "id": 1, "type": "free-text", "text": "Hvad er 3/4 som decimaltal?", "correctAnswer": "0.75", "hint": "Divider 3 med 4" }
```

### 🔢 Potenser og kvadratrødder

```json
{ "id": 1, "type": "free-text", "text": "Hvad er 2⁵?", "correctAnswer": "32", "hint": "2×2×2×2×2" }
```

### 🔢 Regneregler med negative tal

```json
{ "id": 1, "type": "multiple-choice", "text": "Hvad er −3 × (−4)?", "options": ["-12", "-7", "7", "12"], "correctAnswer": "12", "hint": "Minus gange minus giver plus" }
```

### 🔢 Ligninger

```json
{ "id": 1, "type": "free-text", "text": "Løs: 2x + 5 = 15", "correctAnswer": "5", "hint": "2x = 10, x = ?" }
```

### 🔢 Formler

```json
{ "id": 1, "type": "info", "text": "Formlen for hastighed", "content": "<strong>v = s / t</strong><br><br>v = hastighed, s = strækning, t = tid", "correctAnswer": "", "hint": "" }
```

### 📐 Areal og omkreds

```json
{ "id": 1, "type": "free-text", "text": "Hvad er arealet af en cirkel med radius 5? (afrund til hele tal)", "correctAnswer": "79", "hint": "A = π × r² = 3.14 × 25" }
```

### 📐 Rumfang

```json
{ "id": 1, "type": "free-text", "text": "En cylinder har radius 3 og højde 10. Hvad er rumfanget? (afrund til hele tal)", "correctAnswer": "283", "hint": "V = π × r² × h = 3.14 × 9 × 10" }
```

### 📐 Pythagoras (med trekant-tegning)

```json
{
  "id": 1, "type": "free-text", "text": "Find hypotenusen.",
  "graph": { "triangle": { "sideA": 6, "sideB": 8, "angleC": 90, "labels": { "sideA": "6", "sideB": "8", "sideC": "c = ?", "angleC": "90°" } } },
  "correctAnswer": "10", "hint": "c² = 6² + 8²"
}
```

### 📐 Vinkler og vinkelsum

```json
{ "id": 1, "type": "free-text", "text": "En trekant har vinkler på 40° og 75°. Hvad er den tredje vinkel?", "correctAnswer": "65", "hint": "Vinkelsum = 180°" }
```

### 📐 Målestoksforhold

```json
{ "id": 1, "type": "free-text", "text": "Et kort har målestok 1:50000. En strækning er 3 cm på kortet. Hvor lang er den i virkeligheden i km?", "correctAnswer": "1.5", "hint": "3 × 50000 = 150000 cm = ? km" }
```

### 📊 Gennemsnit, median, typetal

```json
{ "id": 1, "type": "free-text", "text": "Find gennemsnittet: 10, 20, 30, 40", "correctAnswer": "25", "hint": "Sum/antal = 100/4" }
```

### 📊 Sandsynlighed

```json
{ "id": 1, "type": "multiple-choice", "text": "To terninger kastes. Hvad er sandsynligheden for summen 7?", "options": ["1/6", "1/9", "1/12", "1/36"], "correctAnswer": "1/6", "hint": "6 gunstige ud af 36 mulige" }
```

### 📈 Lineære funktioner (med graf)

```json
{
  "id": 1, "type": "free-text", "text": "Aflæs hældningen.",
  "graph": { "functions": [{ "expression": "3x-2", "color": "#0d6efd", "label": "f(x) = 3x − 2" }], "xMin": -3, "xMax": 3, "yMin": -8, "yMax": 8 },
  "correctAnswer": "3", "hint": "Hældningen = koefficienten foran x"
}
```

### 📈 Koordinatsystem aflæsning

```json
{ "id": 1, "type": "free-text", "text": "Hvad er f(2) for den viste funktion?",
  "graph": { "functions": [{ "expression": "x^2-1", "color": "#198754" }], "xMin": -4, "xMax": 4, "yMin": -2, "yMax": 10 },
  "correctAnswer": "3", "hint": "Aflæs y-værdien ved x = 2" }
```

---

## Admin & brugersystem

- **Admin-login:** brugernavn `admin`, adgangskode `admin1234`
- Admin kan se alle brugeres resultater under `/Admin/Dashboard`
- Quiz-resultater gemmes automatisk for indloggede brugere
- Brugere oprettes via `/Account/Register`
