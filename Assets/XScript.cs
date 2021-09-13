using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

public class XScript : MonoBehaviour {
    public KMBombModule Module;
    public KMBombInfo Info;
    public bool YMode;
    public Transform Hinge, StartPos, Target;
    public TextMesh text;
    public MeshRenderer Blinker, Screen;
    public KMAudio Audio;
    public KMGameInfo Game;
    public KMSelectable coverSel;

    private bool pressActive = false;
    private int currentCount = 0;

    private static bool first = true;
    private static bool firstX = true;
    private static bool firstY = true;

    private int val = 0;
    private static List<int> vals = new List<int>();
    private bool _solved = false;

    private static readonly bool[][] CODEX = new bool[][] {
        new bool[] { true,  true,  true,  true,  true,  true,  true,  false, false, false, true,  false, false, false, true,  true,  false, false, true,  true,  true,  true,  true,  true,  true },
        new bool[] { true,  false, false, false, false, false, true,  false, true,  true,  true,  false, false, true,  true,  false, true,  false, true,  false, false, false, false, false, true },
        new bool[] { true,  false, true,  true,  true,  false, true,  false, true, false,  true, false, false, false, false,  true,  true, false,  true, false,  true,  true,  true, false,  true },
        new bool[] { true,  false, true,  true,  true,  false, true,  false,false,  true, false,  true,  true, false, false,  true, false, false,  true, false,  true,  true,  true, false,  true },
        new bool[] { true,  false, true,  true,  true,  false, true,  false, true,  true, false,  true,  true,  true, false, false,  true, false,  true, false,  true,  true,  true, false,  true },
        new bool[] { true,  false, false, false, false, false, true,  false, true, false,  true, false,  true, false, false, false, false, false,  true, false, false, false, false, false,  true },
        new bool[] { true,  true,  true,  true,  true,  true,  true,  false, true, false,  true, false,  true, false,  true, false,  true, false,  true,  true,  true,  true,  true,  true,  true },
        new bool[] { false, false, false, false, false, false, false, false, true, false,  true, false, false, true , false, false, false, false, false, false, false, false, false, false, false },
        new bool[] { true,  true, false,  true, false, false,  true,  true, false, false, false,  true,  true,  true,  true, false,  true, false,  true,  true,  true, false,  true,  true, false },
        new bool[] { true,  true,  true, false,  true,  true, false, false, false,  true, false,  true, false,  true,  true, false, false, false,  true, false,  true, false, false,  true, false },
        new bool[] {false,  true,  true, false,  true,  true,  true,  true,  true, false, false, false, false, false, false, false,  true,  true,  true,  true, false,  true,  true, false, false },
        new bool[] {false,  true, false,  true, false,  true, false, false,  true,  true,  true,  true,  true, false,  true, false,  true,  true,  true,  true,  true, false, false, false,  true },
        new bool[] {false, false,  true, false,  true,  true,  true, false, false, false,  true, false,  true, false, false, false, false,  true,  true,  true, false, false,  true,  true,  true },
        new bool[] {false,  true, false, false, false,  true, false,  true, false, false, false,  true,  true, false,  true,  true, false,  true, false,  true,  true, false,  true,  true, false },
        new bool[] { true, false,  true, false, false,  true,  true,  true,  true, false,  true, false,  true,  true, false, false, false, false,  true, false,  true,  true, false, false, false },
        new bool[] {false,  true,  true,  true,  true,  true, false, false,  true, false, false, false,  true, false, false, false,  true,  true,  true,  true, false, false, false, false, false },
        new bool[] { true,  true,  true, false,  true, false,  true, false, false, false,  true, false,  true, false, false, false,  true,  true,  true,  true,  true, false, false, false,  true },
        new bool[] {false, false, false, false, false, false, false, false,  true,  true, false, false,  true,  true, false, false,  true, false, false, false,  true, false, false, false, false },
        new bool[] { true,  true,  true,  true,  true,  true,  true, false,  true,  true,  true, false,  true,  true, false, false,  true, false,  true, false,  true,  true, false,  true,  true },
        new bool[] { true, false, false, false, false, false,  true, false, false,  true,  true, false,  true,  true,  true,  true,  true, false, false, false,  true,  true,  true,  true,  true },
        new bool[] { true, false,  true,  true,  true, false,  true, false, false,  true, false,  true,  true, false, false,  true,  true,  true,  true,  true,  true, false,  true,  true, false },
        new bool[] { true, false,  true,  true,  true, false,  true, false,  true, false,  true,  true,  true,  true,  true, false,  true,  true,  true,  true,  true, false, false, false, false },
        new bool[] { true, false,  true,  true,  true, false,  true, false, false,  true,  true, false, false, false,  true,  true, false,  true,  true,  true, false,  true,  true, false,  true },
        new bool[] { true, false, false, false, false, false,  true, false,  true,  true,  true,  true,  true, false, false,  true,  true,  true, false, false, false,  true, false,  true,  true },
        new bool[] { true,  true,  true,  true,  true,  true,  true, false,  true,  true,  true, false,  true, false,  true,  true, false, false, false, false,  true,  true, false,  true,  true }
    };
    private static readonly bool[][] CODEY = new bool[][] {
        new bool[] { true,  true,  true,  true,  true,  true,  true,  false, false, true , false, true, false, false, false, false,  true, false,  true,  true,  true,  true,  true,  true,  true },
        new bool[] { true,  false, false, false, false, false, true,  false, true,  true, false,  true , true , true , true,  false, false, false, true,  false, false, false, false, false, true },
        new bool[] { true,  false, true,  true,  true,  false, true,  false, false, true, false,  true, false, false, false, false, false, false,  true, false,  true,  true,  true, false,  true },
        new bool[] { true,  false, true,  true,  true,  false, true,  false, true,  true, false, false, false,  true, false,  true, false, false,  true, false,  true,  true,  true, false,  true },
        new bool[] { true,  false, true,  true,  true,  false, true, false, false, false, false, false, false, false,  true,  true,  true, false,  true, false,  true,  true,  true, false,  true },
        new bool[] { true,  false, false, false, false, false, true,  false, true, false,  true,  true,  true,  true,  true, false, false, false,  true, false, false, false, false, false,  true },
        new bool[] { true,  true,  true,  true,  true,  true,  true,  false, true, false,  true, false,  true, false,  true, false,  true, false,  true,  true,  true,  true,  true,  true,  true },
        new bool[] {false, false, false, false, false, false, false, false, false, false, false,  true, false,  true, false, false,  true, false, false, false, false, false, false, false,  false},
        new bool[] { true,  true,  true,  true,  true, false,  true,  true,  true,  true,  true, false, false,  true,  true,  true, false,  true, false,  true, false,  true, false,  true,  false},
        new bool[] {false,  true,  true, false, false,  true, false, false,  true,  true, false, false,  true, false,  true, false, false,  true, false,  true,  true, false, false, false,  true },
        new bool[] { true, false, false,  true,  true,  true,  true, false,  true,  true, false, false, false,  true,  true,  true,  true,  true,  true, false,  true, false,  true, false,  false},
        new bool[] {false,  true,  true,  true,  true,  true, false, false, false,  true,  true,  true, false,  true, false, false,  true,  true, false, false, false, false, false, false,  false},
        new bool[] { true,  true,  true, false, false,  true,  true, false,  true,  true, false, false,  true, false,  true,  true,  true,  true,  true,  true,  true,  true, false,  true,  true },
        new bool[] { true, false,  true,  true, false, false, false, false,  true,  true,  true,  true, false, false,  true,  true,  true, false,  true,  true,  true, false, false, false,  true },
        new bool[] { true, false,  true,  true,  true, false,  true,  true, false, false, false, false,  true,  true, false,  true,  true,  true,  true, false,  true, false,  true,  true,  false},
        new bool[] { true, false, false,  true,  true,  true, false,  true, false,  true, false,  true, false,  true, false, false,  true, false, false, false, false, false, false,  true,  true },
        new bool[] { true, false, false,  true,  true,  true,  true, false,  true, false,  true, false, false,  true,  true,  true,  true,  true,  true,  true,  true,  true, false, false,  true },
        new bool[] {false, false, false, false, false, false, false, false,  true, false, false,  true, false, false,  true, false,  true, false, false, false,  true, false, false, false,  true },
        new bool[] { true,  true,  true,  true,  true,  true,  true, false,  true,  true,  true, false, false,  true,  true,  true,  true, false,  true, false,  true, false,  true,  true,  true },
        new bool[] { true, false, false, false, false, false,  true, false, false, false,  true,  true, false,  true,  true,  true,  true, false, false, false,  true,  true, false, false,  false},
        new bool[] { true, false,  true,  true,  true, false,  true, false,  true,  true, false, false,  true, false, false, false,  true,  true,  true,  true,  true,  true, false, false,  false},
        new bool[] { true, false,  true,  true,  true, false,  true, false,  true, false, false,  true,  true, false,  true, false,  true, false, false, false,  true, false, false,  true,  true },
        new bool[] { true, false,  true,  true,  true, false,  true, false,  true,  true, false, false, false,  true, false, false, false,  true,  true, false,  true, false,  true, false,  true },
        new bool[] { true, false, false, false, false, false,  true, false,  true, false, false,  true, false, false,  true,  true,  true,  true,  true,  true,  true,  true, false,  true,  false},
        new bool[] { true,  true,  true,  true,  true,  true,  true, false,  true, false,  true, false, false,  true,  true, false,  true, false, false, false, false, false,  true,  true,  true }
    };

  private static readonly bool[][] CODESECRETX = new bool[][] {
    new bool[] {true,true,true,true,true,true,true, false,true,true,true,true, false,true,true, false, false, false,true,true,true,true,true,true,true },
    new bool[] {true, false, false, false, false, false,true, false, false, false, false,true,true, false, false,true, false, false,true, false, false, false, false, false,true },
    new bool[] {true, false,true,true,true, false,true, false, false,true, false,true, false,true, false, false,true, false,true, false,true,true,true, false,true },
    new bool[] {true, false,true,true,true, false,true, false, false, false,true,true, false, false,true, false,true, false,true, false,true,true,true, false,true },
    new bool[] {true, false,true,true,true, false,true, false, false, false,true,true,true, false, false,true,true, false,true, false,true,true,true, false,true },
    new bool[] {true, false, false, false, false, false,true, false, false,true, false,true, false,true,true,true,true, false,true, false, false, false, false, false,true },
    new bool[] {true,true,true,true,true,true,true, false,true, false,true, false,true, false,true, false,true, false,true,true,true,true,true,true,true },
    new bool[] { false, false, false, false, false, false, false, false,true,true, false, false,true,true, false,true,true, false, false, false, false, false, false, false, false },
    new bool[] {true,true, false,true,true, false,true, false, false,true, false, false,true,true,true,true,true, false,true, false, false, false, false, false,true },
    new bool[] {true, false,true,true, false, false, false, false,true, false,true, false, false, false,true,true,true,true, false, false,true,true,true, false,true },
    new bool[] { false,true, false, false, false,true,true,true,true,true, false, false,true,true,true,true,true, false,true,true, false, false,true,true, false },
    new bool[] {true, false,true,true,true,true, false, false,true, false, false, false, false,true, false,true, false,true, false,true, false,true,true,true, false },
    new bool[] {true,true, false,true, false, false,true,true,true,true,true, false,true, false, false,true, false,true, false, false, false,true,true, false,true },
    new bool[] {true,true, false, false, false, false, false, false,true,true,true, false,true,true, false, false,true, false,true, false, false,true, false, false,true },
    new bool[] {true,true, false,true,true, false,true,true,true,true,true, false,true, false,true,true, false,true,true,true,true, false, false, false, false },
    new bool[] {true, false,true,true,true, false, false,true, false,true, false, false,true,true,true,true, false, false, false, false,true,true,true,true,true },
    new bool[] {true, false, false, false, false,true,true,true, false,true, false,true, false,true, false,true,true,true,true,true,true,true, false,true,true },
    new bool[] { false, false, false, false, false, false, false, false,true,true,true,true, false, false, false, false,true, false, false, false,true,true,true,true,true },
    new bool[] {true,true,true,true,true,true,true, false, false,true, false,true, false,true, false,true,true, false,true, false,true, false, false, false,true },
    new bool[] {true, false, false, false, false, false,true, false, false,true, false,true,true, false,true,true,true, false, false, false,true, false, false, false, false },
    new bool[] {true, false,true,true,true, false,true, false,true, false, false, false,true,true, false,true,true,true,true,true,true,true,true, false, false },
    new bool[] {true, false,true,true,true, false,true, false,true, false, false,true,true, false,true,true, false,true, false,true, false, false,true,true,true },
    new bool[] {true, false,true,true,true, false,true, false, false, false, false, false, false, false, false, false, false, false, false,true, false, false,true,true,true },
    new bool[] {true, false, false, false, false, false,true, false,true,true,true,true, false,true, false, false, false, false,true, false,true, false, false, false, false },
    new bool[] {true,true,true,true,true,true,true, false,true, false,true,true, false,true,true,true,true, false, false,true,true,true, false, false,true }
  };
  private static readonly bool[][] CODESECRETY = new bool[][] {
    new bool[] {true,true,true,true,true,true,true, false, false, false,true, false, false, false,true,true, false, false,true,true,true,true,true,true,true },
    new bool[] {true, false, false, false, false, false,true, false,true,true,true, false, false,true,true, false,true, false,true, false, false, false, false, false,true },
    new bool[] {true, false,true,true,true, false,true, false,true, false, false, false, false, false, false,true,true, false,true, false,true,true,true, false,true },
    new bool[] {true, false,true,true,true, false,true, false, false,true, false, false,true,true, false,true, false, false,true, false,true,true,true, false,true },
    new bool[] {true, false,true,true,true, false,true, false,true,true,true, false,true,true, false, false,true, false,true, false,true,true,true, false,true },
    new bool[] {true, false, false, false, false, false,true, false,true, false,true, false,true, false, false, false, false, false,true, false, false, false, false, false,true },
    new bool[] {true,true,true,true,true,true,true, false,true, false,true, false,true, false,true, false,true, false,true,true,true,true,true,true,true },
    new bool[] { false, false, false, false, false, false, false, false,true, false,true,true, false, false,true, false, false, false, false, false, false, false, false, false, false },
    new bool[] {true,true, false,true, false, false,true,true, false, false, false,true,true, false,true, false,true, false,true,true,true, false,true,true, false },
    new bool[] {true, false, false,true, false,true, false,true,true,true, false,true,true,true, false, false, false, false,true,true, false, false, false,true, false },
    new bool[] { false, false, false, false,true,true,true,true, false, false, false,true,true, false,true, false,true,true,true, false, false,true,true, false, false },
    new bool[] { false, false,true, false, false, false, false, false,true,true,true,true,true, false,true, false,true, false,true, false,true, false, false, false,true },
    new bool[] {true,true, false,true,true, false,true,true, false, false,true,true,true,true, false, false, false, false, false,true, false, false,true,true,true },
    new bool[] { false,true,true,true,true, false, false,true,true, false, false,true, false, false,true,true, false,true, false,true,true, false,true,true, false },
    new bool[] {true, false,true,true, false, false,true, false, false, false,true,true,true,true,true, false, false, false,true, false,true,true, false,true, false },
    new bool[] { false,true,true, false, false, false, false, false, false, false, false,true, false,true, false, false,true,true,true,true, false, false, false, false, false },
    new bool[] {true,true, false, false,true,true,true,true,true, false, false, false,true, false,true, false,true,true,true,true,true, false, false, false,true },
    new bool[] { false, false, false, false, false, false, false, false,true,true, false, false,true,true,true,true,true, false, false, false,true, false, false, false, false },
    new bool[] {true,true,true,true,true,true,true, false,true, false, false, false, false,true, false, false,true, false,true, false,true,true, false,true,true },
    new bool[] {true, false, false, false, false, false,true, false, false,true, false, false, false, false,true, false,true, false, false, false,true,true,true,true,true },
    new bool[] {true, false,true,true,true, false,true, false, false, false,true,true,true, false, false, false,true,true,true,true,true, false,true,true, false },
    new bool[] {true, false,true,true,true, false,true, false,true, false, false, false,true,true, false, false,true, false,true, false,true,true, false, false, false },
    new bool[] {true, false,true,true,true, false,true, false, false,true, false,true,true,true, false,true, false,true, false, false, false,true,true, false,true },
    new bool[] {true, false, false, false, false, false,true, false,true,true, false, false,true, false,true,true,true,true, false,true, false,true,true,true,true },
    new bool[] {true,true,true,true,true,true,true, false,true, false,true, false,true, false, false, false,true,true, false, false,true, false, false,true,true }
  };

    private static readonly bool[][] Patterns = new bool[][] {
        new bool[] { false, false, false, false, false,   false, true , false, false, false,   false, true , false, false, false,   false, false, false, false, false,   false, false, true , false, false,   false, false, false, false, false },
        new bool[] { false, false, false, false, false,   false, true , false, false, false,   false, false, true , false, false,   false, false, false, false, false,   false, false, false, true , false,   false, false, false, false, false },
        new bool[] { false, false, false, false, false,   false, false, false, false, false,   false, false, true , false, true ,   false, false, false, false, false,   false, false, false, false, true ,   false, false, false, false, false },
        new bool[] { false, false, false, false, false,   false, false, false, false, false,   false, false, false, true , true ,   false, false, false, false, false,   false, false, false, false, false,   true , false, false, false, false },
        new bool[] { true , false, false, false, false,   false, false, false, false, false,   false, false, false, false, true ,   false, false, false, false, false,   false, false, false, false, false,   false, true , false, false, false },

        new bool[] { true , false, false, false, false,   false, false, false, false, false,   false, false, false, false, false,   true , false, false, false, false,   false, false, false, false, false,   false, false, true , false, false },
        new bool[] { false, true , false, false, false,   false, false, false, false, false,   false, false, false, false, false,   true , false, false, false, false,   false, false, false, false, false,   false, false, false, true , false },
        new bool[] { false, true , false, false, false,   false, false, false, false, false,   false, false, false, false, false,   true , false, false, false, false,   false, false, false, false, false,   false, false, false, false, true  },
        new bool[] { false, false, true , false, false,   false, false, true , false, false,   false, false, false, false, false,   true , false, false, false, false,   false, false, false, false, false,   false, false, false, false, false },
        new bool[] { false, false, true , false, false,   false, false, false, true , false,   false, false, false, false, false,   false, true , false, false, false,   false, false, false, false, false,   false, false, false, false, false },

        new bool[] { false, false, false, true , false,   false, false, false, false, false,   false, false, false, false, false,   false, true , false, false, false,   false, false, false, false, false,   false, false, false, false, false },
        new bool[] { false, false, false, false, true ,   false, false, false, false, false,   true , false, false, false, false,   false, true , false, false, false,   false, false, false, false, false,   false, false, false, false, false },
        new bool[] { false, false, false, false, true ,   false, false, false, false, false,   false, true , false, false, false,   false, true , false, false, false,   false, false, true , false, false,   false, false, false, false, false },
        new bool[] { false, false, false, false, false,   true , false, false, false, false,   false, false, true , false, false,   false, false, true , false, false,   false, false, false, true , true ,   false, false, false, false, false },
        new bool[] { false, false, false, false, false,   true , false, false, false, false,   false, false, false, true , false,   false, false, true , false, false,   false, false, false, false, false,   true , false, false, false, false },

        new bool[] { true , false, false, false, false,   false, true , false, false, false,   false, false, false, false, false,   false, false, true , false, false,   false, false, false, false, false,   false, true , true , false, false },
        new bool[] { false, true , false, false, false,   false, true , false, false, false,   false, false, false, false, false,   false, false, false, true , false,   false, false, false, false, false,   false, false, false, true , true  },
        new bool[] { false, false, true , false, false,   false, false, true , true , false,   false, false, false, false, true ,   false, false, false, true , false,   false, false, false, false, false,   false, false, false, false, false },
        new bool[] { false, false, false, true , false,   false, false, false, false, true ,   false, false, false, false, false,   true , false, false, true , false,   false, false, false, false, false,   false, false, false, false, false },
        new bool[] { false, false, false, false, true ,   false, false, false, false, false,   true , true , false, false, false,   true , false, false, true , false,   false, false, false, false, false,   false, false, false, false, false },

        new bool[] { false, false, false, false, false,   true , false, false, false, false,   false, false, true , true , false,   false, true , false, false, true ,   false, false, false, false, false,   false, false, false, false, false },
        new bool[] { true , true , false, false, false,   false, true , false, false, false,   false, false, false, false, false,   false, true , false, false, true ,   false, false, true , true , false,   false, false, false, false, false },
        new bool[] { false, false, true , false, false,   false, false, false, false, false,   false, false, false, false, true ,   false, false, true , false, true ,   false, false, false, false, true ,   true , true , false, false, false },
        new bool[] { false, false, false, true , true ,   false, false, true , false, false,   false, false, false, false, false,   true , false, false, true , true ,   false, false, false, false, false,   false, false, true , true , true  },
        new bool[] { false, false, false, false, false,   true , true , false, true , true ,   true , true , false, false, false,   false, true , false, true , false,   true , false, false, false, false,   false, false, false, false, false },

        new bool[] { true , false, false, false, false,   false, false, false, false, false,   false, false, true , true , true ,   false, false, true , false, true ,   true , false, false, false, false,   false, false, false, false, false },
        new bool[] { false, true , true , true , true ,   false, false, false, false, false,   false, false, false, false, false,   true , true , false, true , true ,   true , false, false, false, false,   false, false, false, false, false },
        new bool[] { false, false, false, false, false,   true , true , false, false, false,   false, false, false, false, true ,   true , false, true , true , true ,   true , false, false, false, false,   false, false, false, false, false },
        new bool[] { false, false, false, false, false,   false, false, false, false, false,   false, false, false, false, false,   false, true , true , true , true ,   true , true , false, false, false,   false, false, false, false, false },
        new bool[] { false, false, false, false, false,   false, false, false, false, false,   false, false, false, false, false,   false, false, false, false, false,   true , true , false, false, false,   false, false, false, false, false }
    };

    private void OnDestroy()
    {
        try
        {
            Application.logMessageReceived -= logd;
            Regex rxDel2 = new Regex("removeimpl", RegexOptions.IgnoreCase);
            MethodInfo del2Handler = tOIH.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).Where(x => rxDel2.IsMatch(x.Name)).First();
            System.Object obj = del2Handler.Invoke(fldOnInt.GetValue(AlarmButton), addHandlerArgs);
            fldOnInt.SetValue(AlarmButton, obj);
        }
        catch
        {
            Debug.Log("Very, very bad error. This module will likely stop working until you restart your game. Please report this ASAP.");
        }
    }

    private Application.LogCallback logd;

    // Use this for initialization
    void Start () {
        val = Random.Range(0, 30);
        StartCoroutine(activate());
        logd = new Application.LogCallback(LogReader);
        Application.logMessageReceived += logd;
        StartCoroutine(Flash());
        coverSel.OnInteract += delegate () { coverSel.AddInteractionPunch(); return false; };
	}
	
	private void HandleLazy()
    {
        if (YMode && !Info.GetSolvableModuleNames().Contains("X"))
        {
            StartCoroutine(Open());
        }
        else if (!YMode && Info.GetSolvableModuleNames().Contains("Y"))
        {
            StartCoroutine(Open());
        }
    }

    private void HandleMash(int count)
    {
        if (count < 3) return;
        if (count == 3)
        {
            if (!YMode && !Info.GetSolvableModuleNames().Contains("Y"))
            {
                StartCoroutine(Open());
            }
            else if (YMode && Info.GetSolvableModuleNames().Contains("X"))
            {
                StartCoroutine(Open());
            }
        }
        else if (count <= 10) { }
        else if (count == val + 11)
        {
            if (!_solved)
            {
                Module.HandlePass();
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                try
                {
                    Application.logMessageReceived -= logd;
                    Regex rxAdd = new Regex("removeimpl", RegexOptions.IgnoreCase);
                    MethodInfo addHandler = tOIH.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).Where(x => rxAdd.IsMatch(x.Name)).First();
                    System.Object obj = addHandler.Invoke(fldOnInt.GetValue(AlarmButton), addHandlerArgs);
                    fldOnInt.SetValue(AlarmButton, obj);
                }
                catch
                {
                    Debug.Log("Very, very bad error. This module will likely stop working until you restart your game. Please report this ASAP.");
                }
            }
        }
        else if (!vals.Contains(count))
        {
            if (!_solved) Module.HandleStrike();
        }
    }

    private IEnumerator Open()
    {
        for (float i = 0f; i < 120f; i++)
        {
            Hinge.localRotation = Quaternion.Slerp(StartPos.localRotation, Target.localRotation, i / 120f);
            yield return null;
        }
    }

    private System.Object[] addHandlerArgs;
    private System.Type tOIH;
    FieldInfo fldOnInt;
    System.Object AlarmButton;
    private bool meFirst = false;
    private static System.Object prevClock = null;

    private IEnumerator activate()
    {
        bool HasTOO = Info.GetSolvableModuleIDs().Contains("theOldOnes");
        if (first)
        {
            vals = new List<int>();
            first = false;
            meFirst = true;
        }
        if (/*firstX && */!YMode)
        {
            char[] ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            bool[][] code = HasTOO ? YMode ? CODESECRETY : CODESECRETX : YMode ? CODEY : CODEX;
            int i = Random.Range(0, code.Length);
            int j = Random.Range(0, code[i].Length);
            Screen.material.color = !code[i][j] ? new Color(1f, 1f, 1f) : new Color(0f, 0f, 0f);
            text.color = code[i][j] ? new Color(1f, 1f, 1f) : new Color(0f, 0f, 0f);
            text.text = ALPHABET[j] + (i + 1).ToString();
            firstX = false;
            StartCoroutine(ResetFirst());
        }
        else if (/*firstY && */YMode)
        {
            char[] ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            bool[][] code = HasTOO ? YMode ? CODESECRETY : CODESECRETX : YMode ? CODEY : CODEX;
            int i = Random.Range(0, code.Length);
            int j = Random.Range(0, code[i].Length);
            Screen.material.color = !code[i][j] ? new Color(1f, 1f, 1f) : new Color(0f, 0f, 0f);
            text.color = code[i][j] ? new Color(1f, 1f, 1f) : new Color(0f, 0f, 0f);
            text.text = ALPHABET[j] + (i + 1).ToString();
            firstY = false;
            StartCoroutine(ResetFirst());
        }
        else
        {
            text.text = "X";
            text.color = new Color(1f, 0f, 0f);
        }
        yield return null;
        if (!meFirst)
        {
            yield return null;
        }
        vals.Add(val + 11);
        try
        {
            Regex rx = new Regex("alarm", RegexOptions.IgnoreCase);
            var components = FindObjectsOfType<GameObject>().Where(x => rx.IsMatch(x.gameObject.name)).Select(x => x.GetComponents<Component>());


            List<Component> Alarms = new List<Component>();
            foreach (var y in components)
            {
                foreach (var w in y)
                {
                    if (w.GetType().Name == "Selectable") Alarms.Add(w);
                }
            }

            System.Object alarm = Alarms[0];

            System.Object[] alarmButtons = (System.Object[])alarm.GetType().GetField("Children").GetValue(alarm);
            AlarmButton = alarmButtons[0];

            Assembly assem = AlarmButton.GetType().Assembly;
            System.Type tSel = assem.GetType(AlarmButton.GetType().Name);
            fldOnInt = tSel.GetField("OnInteract");
            tOIH = fldOnInt.FieldType;
            MethodInfo miHandler = typeof(XScript).GetMethod("HandleOI", BindingFlags.NonPublic | BindingFlags.Static);
            System.Delegate d = System.Delegate.CreateDelegate(tOIH, this, miHandler);

            Regex rxAdd = new Regex("combineimpl", RegexOptions.IgnoreCase);
            MethodInfo addHandler = tOIH.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).Where(x => rxAdd.IsMatch(x.Name)).First();
            addHandlerArgs = new System.Object[] { d };
            if (meFirst)
            {
                System.Object clock = fldOnInt.GetValue(AlarmButton);
                fldOnInt.SetValue(AlarmButton, prevClock);
                prevClock = clock;
            }
            if (fldOnInt.GetValue(AlarmButton) == null)
                fldOnInt.SetValue(AlarmButton, d);
            else
            {
                System.Object obj = addHandler.Invoke(fldOnInt.GetValue(AlarmButton), addHandlerArgs);
                fldOnInt.SetValue(AlarmButton, obj);
            }
            Info.OnBombExploded += delegate ()
            {
                try
                {
                    Application.logMessageReceived -= logd;
                    Regex rxDel = new Regex("removeimpl", RegexOptions.IgnoreCase);
                    MethodInfo delHandler = tOIH.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).Where(x => rxDel.IsMatch(x.Name)).First();
                    System.Object obj = delHandler.Invoke(fldOnInt.GetValue(AlarmButton), addHandlerArgs);
                    fldOnInt.SetValue(AlarmButton, obj);
                }
                catch
                {
                    Debug.Log("Very, very bad error. This module will likely stop working until you restart your game. Please report this ASAP.");
                }
            };
            Info.OnBombSolved += delegate ()
            {
                try
                {
                    Application.logMessageReceived -= logd;
                    Regex rxDel2 = new Regex("removeimpl", RegexOptions.IgnoreCase);
                    MethodInfo del2Handler = tOIH.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).Where(x => rxDel2.IsMatch(x.Name)).First();
                    System.Object obj = del2Handler.Invoke(fldOnInt.GetValue(AlarmButton), addHandlerArgs);
                    fldOnInt.SetValue(AlarmButton, obj);
                }
                catch
                {
                    Debug.Log("Very, very bad error. This module will likely stop working until you restart your game. Please report this ASAP.");
                }
            };
            Game.OnStateChange += delegate (KMGameInfo.State state)
            {
                try
                {
                    Application.logMessageReceived -= logd;
                    Regex rxDel2 = new Regex("removeimpl", RegexOptions.IgnoreCase);
                    MethodInfo del2Handler = tOIH.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).Where(x => rxDel2.IsMatch(x.Name)).First();
                    System.Object obj = del2Handler.Invoke(fldOnInt.GetValue(AlarmButton), addHandlerArgs);
                    fldOnInt.SetValue(AlarmButton, obj);
                }
                catch
                {
                    Debug.Log("Very, very bad error. This module will likely stop working until you restart your game. Please report this ASAP.");
                }
            };
        }
        catch
        {
            Debug.Log("Fatal error, opening cover. Pressing the cover will solve.");
            try
            {
                Application.logMessageReceived -= logd;
                Regex rxAdd = new Regex("removeimpl", RegexOptions.IgnoreCase);
                MethodInfo addHandler = tOIH.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).Where(x => rxAdd.IsMatch(x.Name)).First();
                System.Object obj = addHandler.Invoke(fldOnInt.GetValue(AlarmButton), addHandlerArgs);
                fldOnInt.SetValue(AlarmButton, obj);
            }
            catch
            {
                Debug.Log("Very, very bad error. This module will likely stop working until you restart your game. Please report this ASAP.");
            }
            StartCoroutine(Open());
            text.text = "X";
            text.color = new Color(1f, 0f, 0f);
            Screen.material.color = new Color(0f, 0f, 0f);
            GetComponent<KMSelectable>().Children[0].OnInteract += delegate ()
            {
                Module.HandlePass();
                return false;
            };
        }
    }

    private IEnumerator ResetFirst()
    {
        yield return new WaitForSeconds(0.2f);
        firstX = true;
        firstY = true;
        first = true;
    }

    private static bool HandleOI(XScript module)
    {
        try
        {
            module.currentCount++;
            module.StartCoroutine(module.PressActive(module.currentCount));
        }
        finally { }
        return false;
    }

    private IEnumerator PressActive(int count)
    {
        pressActive = true;
        yield return null;
        yield return null;
        yield return null;
        pressActive = false;
        yield return new WaitForSeconds(2f);
        if (count != currentCount) yield break;
        HandleMash(currentCount);
        currentCount = 0;
    }

    Regex ALMOFF = new Regex(@"\[AlarmClock\] TurnOff", RegexOptions.IgnoreCase);

    private void LogReader(string condition, string stacktrace, LogType type)
    {
        if (!ALMOFF.IsMatch(condition)) return;
        StartCoroutine(WaitForPress());
    }

    private IEnumerator WaitForPress()
    {
        for (int i = 0; i < 3; i++)
        {
            if (pressActive) yield break;
            yield return null;
        }
        HandleLazy();
    }

    private IEnumerator Flash()
    {
        int prevTime = (int)Mathf.Floor(Info.GetTime());
        while (true)
        {
            for (int i = 0; i < Patterns.Length; i++)
            {
                Blinker.material.color = (YMode ? Patterns[i][val] : Patterns[val][i]) ? new Color(1f, 1f, 1f) : new Color(0f, 0f, 0f);
                yield return new WaitWhile(() => Mathf.Abs(Mathf.Floor(Info.GetTime()) - prevTime) < 1);
                prevTime = (int)Mathf.Floor(Info.GetTime());
            }
        }
    }
}
