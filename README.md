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

- **Miscare libera** pe harta jocului (atat pe uscat cat si in apa)
- **Sistem de iteme**: fiecare item are damage propriu
- **Colectare si folosire iteme** (hotbar functional)
- **Interactiune cu plante** (2 tipuri): se pot distruge si ofera drop-uri
- **Plantare personalizata**: drop-urile pot fi plantate doar pe anumite terenuri
- **Feedback audio**: sunete pentru colectare, selectare, atac
- **Interfata minimalista**: bara de iteme jos pe ecran (stil Minecraft)

## Demo and Screenshots
<img src="https://github.com/user-attachments/assets/89f2e7e3-1e6b-4191-9d6a-417ec0c372ae" width="400" alt="Meadows Screenshot 1">
<img src="https://github.com/user-attachments/assets/4bd58e7e-bd26-4871-851f-dff1cbdee2f8" width="400" alt="Meadows Screenshot 2">
<img src="https://github.com/user-attachments/assets/16eee412-0ea6-492f-a17c-b2fe314c022b" width="400" alt="Meadows Screenshot 3">
<img src="https://github.com/user-attachments/assets/55b89b0f-b551-451a-9104-e8e9764882c1" width="400" alt="Meadows Screenshot 4">
<img src="https://github.com/user-attachments/assets/02e31017-6abc-4afb-b107-91534923b866" width="400" alt="Meadows Screenshot 5">
<img src="https://github.com/user-attachments/assets/a279e95c-33d5-4534-a808-22c64470e549" width="400" alt="Meadows Screenshot 6">
<img src="https://github.com/user-attachments/assets/48c7aa48-8fdd-4dc8-bab4-2c7c1f4e3cf6" width="400" alt="Meadows Screenshot 7">
<img src="https://github.com/user-attachments/assets/18b0364f-b96d-4602-b517-02aee48e2cfb" width="400" alt="Meadows Screenshot 8">
<img src="https://github.com/user-attachments/assets/746f4ad8-f605-46fa-91b1-2dcc389c6b02" width="400" alt="Meadows Screenshot 9">
<img src="https://github.com/user-attachments/assets/b8e6dc7f-7861-435d-8f04-6557b5fdc932" width="400" alt="Meadows Screenshot 10">
<img src="https://github.com/user-attachments/assets/9c74131a-f682-4755-9604-ec85cb8b4205" width="400" alt="Meadows Screenshot 11">
<img src="https://github.com/user-attachments/assets/eb98e94b-2c42-4123-9928-2cda9b1fba8e" width="400" alt="Meadows Screenshot 12">
<img src="https://github.com/user-attachments/assets/158011d7-e49c-47ff-bfac-930d06ff036c" width="400" alt="Meadows Screenshot 13">


## 👨‍💻 Autori
- Mușat Fabian
- Poenaru Andrei
- Rădoi Dragoș
- Răican Mihai
- Avrămescu Isaac
