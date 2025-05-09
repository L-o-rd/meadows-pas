# 🌾 Meadows

**Meadows** este un joc 2D sandbox dezvoltat in **C#** folosind framework-ul **MonoGame**, care imbina explorarea naturala cu interactiunea sistemica intr-un mediu stilizat pixel art. Proiectul are ca scop recrearea unui ecosistem simplificat in care jucatorul poate naviga, colecta resurse, interactiona cu plante si modifica mediul inconjurator prin actiuni directe.

## Concept

**Obiectivul jocului** este de a oferi o experienta simpla, in care jucatorul:
- Exploreaza liber o harta alcatuita din terenuri variate (iarba, apa, plaja, stanci).
- Colecteaza iteme cu caracteristici unice (ex: damage diferit).
- Interactioneaza cu plante — le poate distruge si planta din nou.
- Foloseste o bara de iteme (hotbar) pentru management rapid al resurselor.
Proiectul serveste si ca **demonstratie practica** a folosirii paradigmelor de programare orientata pe componente, a utilizarii unui game loop personalizat, precum si a manipularii resurselor media prin MonoGame Content Pipeline.

## Tehnologii si librarii utilizate

| Tehnologie                   | Descriere                                                   |
| ---------------------------- | ----------------------------------------------------------- |
| **C#**                       | Limbajul de baza pentru logica jocului si implementarea OOP |
| **MonoGame**                 | Framework open-source pentru jocuri 2D/3D bazat pe XNA      |
| **.mgcb** (Content Pipeline) | Conversia asset-urilor media (sprite-uri, sunete, fonturi)  |
| **CSV**                      | Structura nivelurilor prin fisiere usor editabile           |
| **SpriteFont**               | Randarea textului in joc (fonturi pixelate personalizate)   |

## Functionalitati implementate

- **Mişcare libera** pe harta jocului (atat pe uscat cat si in apa)
- **Sistem de iteme**: fiecare item are damage propriu
- **Colectare si folosire iteme** (hotbar functional)
- **Interactiune cu plante** (2 tipuri): se pot distruge si ofera drop-uri
- **Plantare personalizata**: drop-urile pot fi plantate doar pe anumite terenuri
- **Feedback audio**: sunete pentru colectare, selectare, atac
- **Interfata minimalista**: bara de iteme jos pe ecran (stil Minecraft)

## Demo and Screenshots

_Jucatorul navigheaza prin harta si colecteaza iteme_
 
_Plantare manuala a unei plante distruse anterior_
 
_Bara de iteme functionala in partea de jos a ecranului_

## 👨‍💻 Autori
