using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    public Tilemap tilemap; // Verweise die Tilemap im Inspector
    public TileBase floorTile; // Boden-Tile
    public TileBase wallTile; // Wand-Tile

    public TileBase door;
    public int width = 36; // Feste Breite
    public int height = 20; // Feste Höhe

    public int startX;
    public int startY;
    public int endX;
    public int endY;

    private HashSet<Vector2Int> pathPositions = new HashSet<Vector2Int>();

    void Start()
    {
        AdjustWidthToCamera();
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        // Clear previous dungeon
        tilemap.ClearAllTiles();
        pathPositions.Clear();

        // Random Walk: Plattformen von links nach rechts
        GenerateDungeonRoom();

        // Set Floor Tiles für die Plattformen
        foreach (var pos in pathPositions)
        {
            tilemap.SetTile((Vector3Int)pos, floorTile);
        }

        // Add Wände und restliche Dungeonfläche
        AddWallsAndFill();
    }

    void GenerateDungeonRoom()
    {
        startX = Random.Range((-width / 2) + 3, (width / 2) - 3);
        startY = Random.Range((-height / 2) + 1, (height / 2) - 5);

        // Variablen für die Grenzen des gesperrten Rechtecks
        int restrictedMinX, restrictedMaxX, restrictedMinY, restrictedMaxY;

        // Grenzen basierend auf der Position des Startpunkts festlegen
        if (startX < -11 && startY < -5)
        {
            // Rechteck geht nach rechts und oben
            restrictedMinX = startX;
            restrictedMaxX = startX + 10;
            restrictedMinY = startY;
            restrictedMaxY = startY + 10;
        }
        else if (startX < -11 && startY > 5)
        {
            // Rechteck geht nach rechts und unten
            restrictedMinX = startX;
            restrictedMaxX = startX + 10;
            restrictedMinY = startY - 10;
            restrictedMaxY = startY;
        }
        else if (startX > 11 && startY < -5)
        {
            // Rechteck geht nach links und oben
            restrictedMinX = startX - 10;
            restrictedMaxX = startX;
            restrictedMinY = startY;
            restrictedMaxY = startY + 10;
        }
        else if (startX > 11 && startY > 5)
        {
            // Rechteck geht nach links und unten
            restrictedMinX = startX - 10;
            restrictedMaxX = startX;
            restrictedMinY = startY - 10;
            restrictedMaxY = startY;
        }
        else
        {
            // Standardrechteck: Symmetrisch um den Startpunkt
            restrictedMinX = startX - 4;
            restrictedMaxX = startX + 4;
            restrictedMinY = startY - 5;
            restrictedMaxY = startY + 5;
        }

        // Endposition generieren
        int endX, endY;

        do
        {
            endX = Random.Range((-width / 2) + 3, (width / 2) - 3);
            endY = Random.Range((-height / 2) + 1, (height / 2) - 5);
            endX = Random.Range((-width / 2) + 3, (width / 2) - 3);
            endY = Random.Range((-height / 2) + 1, (height / 2) - 5);
        }
        // Wiederhole, falls die Position im verbotenen Bereich liegt
        while (endX >= restrictedMinX && endX <= restrictedMaxX && endY >= restrictedMinY && endY <= restrictedMaxY);
        




        Vector2Int currentPosition = new Vector2Int(-width / 2, Random.Range(-height / 2 + 1, height / 2 - 1)); // Startposition
        pathPositions.Add(currentPosition);

        // Erzeuge Plattformen von links nach rechts
        while (currentPosition.x < width / 2 - 1)
        {
            // Zufällige Richtung: rechts, hoch, runter
            Vector2Int[] directions = { Vector2Int.right, Vector2Int.up, Vector2Int.down };
            Vector2Int nextStep = directions[Random.Range(0, directions.Length)];

            // Neue Position berechnen
            Vector2Int newPosition = currentPosition + nextStep;

            // Sicherstellen, dass die neue Position innerhalb der Grenzen bleibt
            if (newPosition.y > -height / 2 && newPosition.y < height / 2)
            {
                currentPosition = newPosition;
                pathPositions.Add(currentPosition);
            }
            else if (nextStep == Vector2Int.right)
            {
                // Immer Fortschritt nach rechts sicherstellen
                currentPosition += Vector2Int.right;
                pathPositions.Add(currentPosition);
            }

            // Plattformen mit zufälliger Höhe generieren
            if (Random.Range(0, 3) == 0) // Bestimmt die Häufigkeit, mit der eine neue Plattform erzeugt wird
            {
                int platformHeight = Random.Range(3, 7); // Zufällige Höhe der Plattform
                for (int i = 0; i < platformHeight; i++)
                {
                    Vector2Int platformPosition = new Vector2Int(currentPosition.x, currentPosition.y + Random.Range(-1, 2)); // Zufällige Höhe der Plattform
                    pathPositions.Add(platformPosition);
                }
            }
        }
    }

    void AddWallsAndFill()
    {
        for (int x = -width / 2; x < width / 2; x++)
        {
            for (int y = -height / 2; y < height / 2; y++)
            {
                Vector2Int position = new Vector2Int(x, y);

                // Boden an den Stellen des Pfads
                if (pathPositions.Contains(position))
                {
                    continue;
                }

                // Wände oben und unten
                if (y == -height / 2 || y == height / 2 - 1)
                {
                    tilemap.SetTile((Vector3Int)position, wallTile);
                }
                else
                {
                    // Füllung mit Wänden oder leer
                    tilemap.SetTile((Vector3Int)position, wallTile);
                }
            }
        }
    }

    void AdjustWidthToCamera()
    {
        float aspectRatio = Camera.main.aspect; // Seitenverhältnis der Kamera
        float cameraWidth = 2 * Camera.main.orthographicSize * aspectRatio;

        // Breite auf nächste gerade Zahl runden
        int requiredWidth = Mathf.CeilToInt(cameraWidth);
        if (requiredWidth % 2 != 0) requiredWidth++;

        width = requiredWidth; // Dungeon-Breite setzen
    }
}
