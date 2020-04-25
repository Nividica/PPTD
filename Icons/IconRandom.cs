using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Vec2Int
{
  public int x;

  public int y;

  public Vec2Int(int x, int y)
  {
    this.x = x;
    this.y = y;
  }
  public Vec2Int(Vector2 source)
  {
    this.x = (int)source.x;
    this.y = (int)source.y;
  }
  public Vec2Int(Vec2Int source)
  {
    this.x = source.x;
    this.y = source.y;
  }
}

public class SearchState
{
  public Vec2Int pos;
  public int direction;

  public SearchState(Vec2Int pos, int dir = 0)
  {
    this.pos = pos;
    this.direction = dir;
  }
}

public class GridMap<T>
{
  private Dictionary<int, Dictionary<int, T>> backingMap = new Dictionary<int, Dictionary<int, T>>();

  public T get(int x, int y)
  {
    if (backingMap.ContainsKey(x))
    {
      var row = backingMap[x];
      if (row.ContainsKey(y))
      {
        return row[y];
      }
    }

    return default(T);
  }

  public void set(int x, int y, T value)
  {
    (backingMap.ContainsKey(x)
      ? backingMap[x]
      : backingMap[x] = new Dictionary<int, T>()
    )[y] = value;
  }
}

public class IconRandom : TileMap
{
  private Vec2Int entrancePos;

  private List<Vec2Int> exitPath;

  private Vec2Int baddiePosition;

  private int baddiePathIndex = 0;

  private int baddieTileID;

  private float deltaAcc = 0;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    int entranceID = this.TileSet.FindTileByName("Entrance");
    int exitID = this.TileSet.FindTileByName("Exit");
    this.baddieTileID = this.TileSet.FindTileByName("Baddie");
    // Find entrance
    foreach (Vector2 vec in this.GetUsedCells())
    {
      if (this.GetCell((int)vec.x, (int)vec.y) == entranceID)
      {
        // Found it!
        this.entrancePos = new Vec2Int(vec);
        break;
      }
    }

    // Get the path tilemap
    TileMap thePath = (TileMap)this.GetNode(new NodePath("../Path"));

    // Get path to exit
    this.exitPath = this.pathTo(thePath, this.entrancePos, (cellID) => cellID == exitID);
    this.baddiePosition = this.exitPath[this.baddiePathIndex];
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
    this.deltaAcc -= delta;
    Console.WriteLine(delta);
    if (this.deltaAcc <= 0)
    {
      this.deltaAcc = 0.125F;

      // Remove baddie
      this.SetCell(this.baddiePosition.x, this.baddiePosition.y, -1);

      // Step
      if (++this.baddiePathIndex == this.exitPath.Count)
      {
        this.baddiePathIndex = 0;
      }
      this.baddiePosition = this.exitPath[this.baddiePathIndex];

      // Draw baddie
      this.SetCell(this.baddiePosition.x, this.baddiePosition.y, this.baddieTileID);
    }
  }

  private List<Vec2Int> pathTo(TileMap pathMap, Vec2Int start, Predicate<int> isMatch)
  {
    GridMap<bool> visited = new GridMap<bool>();
    Stack<SearchState> path = new Stack<SearchState>();
    path.Push(new SearchState(start));

    while (path.Count > 0)
    {
      Console.WriteLine();
      Console.WriteLine("Peek");
      SearchState cell = path.Peek();
      Console.WriteLine("Cell: ({0},{1}):{2}", cell.pos.x, cell.pos.y, cell.direction);
      // First visit?
      if (cell.direction == 0)
      {
        // Visited or Not path?
        if (visited.get(cell.pos.x, cell.pos.y) || pathMap.GetCell(cell.pos.x, cell.pos.y) == -1)
        {
          // Ignore
          Console.WriteLine("Visited Or Not Path:: POP");
          path.Pop();
          continue;
        }

        // Match?
        int thisCellID = this.GetCell(cell.pos.x, cell.pos.y);
        if (isMatch(thisCellID))
        {
          // Found it!
          Console.WriteLine("Found Match");
          return path.Select((ss) => ss.pos).Reverse().ToList();
        }
        else if (thisCellID != -1 && path.Count > 1)
        {
          // Blocked
          Console.WriteLine("Blocked:: POP");
          path.Pop();
          continue;
        }

        // Mark visited
        Console.WriteLine("Marking Visited");
        visited.set(cell.pos.x, cell.pos.y, true);
      }

      // Determine which neighbor to visit next
      Vec2Int neighborPos = new Vec2Int(cell.pos);
      switch (cell.direction)
      {
        // Up
        case 0:
          neighborPos.y--;
          break;
        // Down
        case 1:
          neighborPos.y++;
          break;
        // Left
        case 2:
          neighborPos.x--;
          break;
        // Right
        case 3:
          neighborPos.x++;
          break;

        default:
          // No more neighbors, this cell is dead end
          path.Pop();
          continue;
      }
      cell.direction = cell.direction + 1;

      Console.WriteLine("Push Neighbor");
      Console.WriteLine("Cell:: ({0},{1}):{2}", cell.pos.x, cell.pos.y, cell.direction);
      Console.WriteLine("Neighbor:: ({0},{1})", neighborPos.x, neighborPos.y);
      Console.WriteLine();

      // Push neighbor
      path.Push(new SearchState(neighborPos));
    }

    return new List<Vec2Int>();
  }
}
