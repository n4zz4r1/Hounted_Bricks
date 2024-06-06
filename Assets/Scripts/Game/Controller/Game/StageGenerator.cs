using System.Collections.Generic;
using System.Linq;
using Core.Utils.Constants;
using Random = System.Random;
using Monster = Core.Utils.Constants.Monsters.Monster;
using MonsterBoss = Core.Utils.Constants.Monsters.MonsterBoss;


namespace Game.Controller.Game {
public abstract class StageGenerator {
    // Set monsters accordingly with its level and difficulty
    // TODO set real probability
    public static Dictionary<StageLevelType, Dictionary<StageDifficultyType, List<Monster>>>
        monstersByLevelAndDifficulty = new() {
            {
                StageLevelType.GARDEN, new Dictionary<StageDifficultyType, List<Monster>> {
                    { StageDifficultyType.EASY, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.MEDIUM, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.HARD, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.IMPOSSIBLE, new() { Monster.M01_Ghost_Green } }
                }
            }, {
                StageLevelType.KITCHEN, new Dictionary<StageDifficultyType, List<Monster>> {
                    { StageDifficultyType.EASY, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.MEDIUM, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.HARD, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.IMPOSSIBLE, new() { Monster.M01_Ghost_Green } }
                }
            }, {
                StageLevelType.LIVING_ROOM, new Dictionary<StageDifficultyType, List<Monster>> {
                    { StageDifficultyType.EASY, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.MEDIUM, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.HARD, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.IMPOSSIBLE, new() { Monster.M01_Ghost_Green } }
                }
            }, {
                StageLevelType.REST_ROOM, new Dictionary<StageDifficultyType, List<Monster>> {
                    { StageDifficultyType.EASY, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.MEDIUM, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.HARD, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.IMPOSSIBLE, new() { Monster.M01_Ghost_Green } }
                }
            }, {
                StageLevelType.BED_ROOM, new Dictionary<StageDifficultyType, List<Monster>> {
                    { StageDifficultyType.EASY, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.MEDIUM, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.HARD, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.IMPOSSIBLE, new() { Monster.M01_Ghost_Green } }
                }
            }, {
                StageLevelType.CRYPT, new Dictionary<StageDifficultyType, List<Monster>> {
                    { StageDifficultyType.EASY, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.MEDIUM, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.HARD, new() { Monster.M01_Ghost_Green } },
                    { StageDifficultyType.IMPOSSIBLE, new() { Monster.M01_Ghost_Green } }
                }
            }
        };

    public static StageLevelType GetStageTypeByLevel(int level) {
        var levelBy120 = level % 120;
        return levelBy120 switch {
            <= 20 => StageLevelType.GARDEN,
            <= 40 => StageLevelType.KITCHEN,
            <= 60 => StageLevelType.LIVING_ROOM,
            <= 80 => StageLevelType.REST_ROOM,
            <= 100 => StageLevelType.BED_ROOM,
            _ => StageLevelType.CRYPT
        };
    }

    public static StageDifficultyType GetDifficultyByLevel(int level) {
        var levelBy20 = level % 20;
        return levelBy20 switch {
            5 or 10 or 15 or 17 => StageDifficultyType.MEDIUM,
            0 => StageDifficultyType.HARD,
            _ => StageDifficultyType.EASY
        };
    }

    public static int GetNumberOfMonstersOnWave(int level) {
        Random random = new();
        return level switch {
            <= 25 => random.Next(1, 3),
            <= 50 => random.Next(2, 5),
            <= 75 => random.Next(4, 5),
            _ => random.Next(4, 6)
        };
    }

    public static List<int> ShuffleList(int count) {
        var numbers = new List<int> { 0, 1, 2, 3, 4, 5 };
        var random = new Random();
        for (var i = numbers.Count - 1; i > 0; i--) {
            var swapIndex = random.Next(i + 1);
            (numbers[i], numbers[swapIndex]) = (numbers[swapIndex], numbers[i]);
        }

        return numbers.Take(count).ToList();
    }

    public static int ShuffleListBoss() {
        var numbers = new List<int> { 0, 1, 2, 3, 4 };
        var random = new Random();
        for (var i = numbers.Count - 1; i > 0; i--) {
            var swapIndex = random.Next(i + 1);
            (numbers[i], numbers[swapIndex]) = (numbers[swapIndex], numbers[i]);
        }

        return numbers[0];
    }

    // TODO improve here once balancing and monsters
    public static Monster ChooseMonster(int level, List<Monster> easy, List<Monster> mid, List<Monster> hard) {
        return Monster.M01_Ghost_Green;
    }

    // Define if stage has boss or not
    public static MonsterBoss GetBossFromLevel(int level) {
        var rnd = level % 120;
        return level switch {
            0 => MonsterBoss.B07_Crypt,
            20 => MonsterBoss.B02_Garden,
            40 => MonsterBoss.B04_Kitchen,
            60 => MonsterBoss.B03_LivingRoom,
            80 => MonsterBoss.B05_Bathroom,
            100 => MonsterBoss.B06_BedRoom,
            _ => level % 5 == 0 ? MonsterBoss.B01_Mini_Green : MonsterBoss.NONE
        };
    }

    // Define Number of waves based on the level and probability
    public static int GetNumberOfWavesByLevel(int level) {
        var rnd = new Random();
        return level switch {
            <= 25 => rnd.Next(10, 12),
            <= 50 => rnd.Next(12, 14),
            <= 75 => rnd.Next(14, 16),
            _ => rnd.Next(16, 18)
        };
    }
}
}