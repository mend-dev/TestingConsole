using MelonLoader;
using BTD_Mod_Helper;
using TestingConsole;
using UnityEngine;
using System;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System.Linq;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity;

[assembly: MelonInfo(typeof(TestingConsole.TestingConsole), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace TestingConsole;

public class TestingConsole : BloonsTD6Mod {

    public static TestingConsole mod;
    public bool consoleOpen = false;

    public override void OnApplicationStart() {
        ModHelper.Msg<TestingConsole>("TestingConsole loaded!");
        mod = this;
    }

    public override void OnUpdate() {
        if (InGame.instance == null || InGame.instance.bridge == null) { return; }
        InGame game = InGame.instance;

        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            RectTransform rect = game.uiRect;
            if (consoleOpen) {
                ConsoleUi.instance.CloseConsole();
                consoleOpen = false;
            } else {
                ConsoleUi.CreateConsole(rect);
                consoleOpen = true;
            }
        }
    }
}

[RegisterTypeInIl2Cpp(false)]
public class ConsoleUi : MonoBehaviour {

    public static ConsoleUi instance;
    public ModHelperInputField input;

    public void SubmitCommand() {
        if (InGame.instance == null || InGame.instance.bridge == null) { return; }
        InGame game = InGame.instance;

        string value = input.CurrentValue.Substring(0, input.CurrentValue.Length - 1);
        string[] values = value.Split().Where(x => x != string.Empty).ToArray();
        ModHelper.Msg<TestingConsole>(value);

        string command = values[0].ToLower();

        if (command == "bloon") {
            string subCommand = values[1].ToLower();

            if (subCommand == "spawn") {
                string bloonId = values[2];
                int amount = int.Parse(values[3]);
                int spacing = int.Parse(values[4]);

                string adjustedBloonId = char.ToUpper(bloonId.First()) + bloonId.Substring(1).ToLower();

                game.SpawnBloons(adjustedBloonId, amount, spacing);
            } else if (subCommand == "clear") {
                game.DeleteAllBloons();
            } else if (subCommand == "clearall") { }

        } else if (command == "tower") {
            string subCommand = values[1].ToLower();

            if (subCommand == "spawn") {
                TowerModel tower = GetTower(values[2]);
                int xPos = int.Parse(values[3]);
                int yPos = int.Parse(values[4]);

                game.bridge.CreateTowerAt(new Vector2(xPos, yPos), tower, new Il2CppAssets.Scripts.ObjectId(), false, null);
            }

        } else if (command == "cash") {
            if (values.Length < 2) { return; }
            if (int.TryParse(values[1], out _)) {
                int amount = int.Parse(values[1]);
                game.SetCash(amount);
            } else {
                if (values.Length < 3) { return; }
                string subCommand = values[1].ToLower();
                int amount = 0;
                int.TryParse(values[2], out amount);
                if (subCommand == "add") {
                    game.SetCash(game.GetCash() + amount);
                } else if (subCommand == "remove") {
                    game.SetCash(game.GetCash() - amount);
                } else if (subCommand == "set") {
                    game.SetCash(amount);
                } else if (subCommand == "reset") {
                    game.SetCash(650);
                }
            }
        } else if (command == "lives") {
            if (values.Length < 2) { return; }
            if (int.TryParse(values[1], out _)) {
                int amount = int.Parse(values[1]);
                game.SetHealth(amount);
                game.SetMaxHealth(game.GetHealth());
            } else {
                if (values.Length < 3) { return; }
                string subCommand = values[1].ToLower();
                int amount = 0;
                int.TryParse(values[2], out amount);
                if (subCommand == "add") {
                    game.SetHealth(game.GetHealth() + amount);
                    game.SetMaxHealth(game.GetHealth());
                } else if (subCommand == "remove") {
                    game.SetHealth(game.GetHealth() - amount);
                    game.SetMaxHealth(game.GetHealth());
                } else if (subCommand == "set") {
                    game.SetMaxHealth(amount);
                    game.SetHealth(amount);
                } else if (subCommand == "reset") {
                    game.SetMaxHealth(100);
                    game.SetHealth(game.GetMaxHealth());
                }
            }
        } else if (command == "round") {
            if (values.Length < 2) { return; }
            if (int.TryParse(values[1], out _)) {
                int amount = int.Parse(values[1]);
                game.SetRound(amount);
            } else {
                if (values.Length < 3) { return; }
                string subCommand = values[1].ToLower();
                int amount = 0;
                int.TryParse(values[2], out amount);
                if (subCommand == "add") {
                    game.SetRound(game.bridge.GetCurrentRound() + amount);
                } else if (subCommand == "remove") {
                    game.SetRound(game.bridge.GetCurrentRound() - amount);
                } else if (subCommand == "set") {
                    game.SetRound(amount);
                } else if (subCommand == "reset") {
                    game.SetRound(0);
                }
            }
        } else if (command == "fast") {
        }

        // Bloon
        // Tower
        // Cash
        // Lives
        // Round
    }

    public void CloseConsole() {
        TestingConsole.mod.consoleOpen = false;
        Destroy(gameObject);
    }

    public static void CreateConsole(RectTransform rect) {
        ModHelperPanel panel = rect.gameObject.AddModHelperPanel(new Info("Panel", 500, 150, 825, 125, new Vector2()), VanillaSprites.BrownInsertPanel);
        ConsoleUi consoleUi = panel.AddComponent<ConsoleUi>();
        instance = consoleUi;
        
        consoleUi.input = panel.AddInputField(new Info("Input Field", 0, 0, 800, 100), "", VanillaSprites.BrownInsertPanelDark);
        ModHelperButton submitButton = panel.AddButton(new Info("Submit Button", 450, 0, 100), VanillaSprites.GreenBtn, new Action(() => consoleUi.SubmitCommand()));
        ModHelperButton closeButton = panel.AddButton(new Info("Close Button", 550, 0, 100), VanillaSprites.RedBtn, new Action(() => consoleUi.CloseConsole()));
    }

    private string ConvertBloonId(string id) {
        if (id == "red") {
            return "Red";
        } else {
            return "Red";
        }
    }

    private TowerModel GetTower(string id, int top = 0, int mid = 0, int bot = 0) {
        id = id.ToLower();
        if (id == "dart" || id.Contains("dartmonkey")) {
            return Game.instance.model.GetTower(TowerType.DartMonkey, top, mid, bot);
        } else if (id.Contains("boomer")) {
            return Game.instance.model.GetTower(TowerType.BoomerangMonkey, top, mid, bot);
        } else if (id.Contains("bomb")) {
            return Game.instance.model.GetTower(TowerType.BombShooter, top, mid, bot);
        } else if (id.Contains("tack")) {
            return Game.instance.model.GetTower(TowerType.TackShooter, top, mid, bot);
        } else if (id.Contains("ice")) {
            return Game.instance.model.GetTower(TowerType.IceMonkey, top, mid, bot);
        } else if (id.Contains("glue")) {
            return Game.instance.model.GetTower(TowerType.GlueGunner, top, mid, bot);
        } else if (id.Contains("sniper")) {
            return Game.instance.model.GetTower(TowerType.SniperMonkey, top, mid, bot);
        } else if (id.Contains("sub")) {
            return Game.instance.model.GetTower(TowerType.MonkeySub, top, mid, bot);
        } else if (id.Contains("buccaneer") || id.Contains("boat")) {
            return Game.instance.model.GetTower(TowerType.MonkeyBuccaneer, top, mid, bot);
        } else if (id.Contains("ace")) {
            return Game.instance.model.GetTower(TowerType.MonkeyAce, top, mid, bot);
        } else if (id.Contains("heli") || id.Contains("pilot")) {
            return Game.instance.model.GetTower(TowerType.HeliPilot, top, mid, bot);
        } else if (id.Contains("mortar")) {
            return Game.instance.model.GetTower(TowerType.MortarMonkey, top, mid, bot);
        } else if (id.Contains("dartling")) {
            return Game.instance.model.GetTower(TowerType.DartlingGunner, top, mid, bot);
        } else if (id.Contains("wizard")) {
            return Game.instance.model.GetTower(TowerType.WizardMonkey, top, mid, bot);
        } else if (id.Contains("super")) {
            return Game.instance.model.GetTower(TowerType.SuperMonkey, top, mid, bot);
        } else if (id.Contains("ninja")) {
            return Game.instance.model.GetTower(TowerType.NinjaMonkey, top, mid, bot);
        } else if (id.Contains("alchemist")) {
            return Game.instance.model.GetTower(TowerType.Alchemist, top, mid, bot);
        } else if (id.Contains("druid")) {
            return Game.instance.model.GetTower(TowerType.Druid, top, mid, bot);
        } else if (id.Contains("farm") || id.Contains("banana")) {
            return Game.instance.model.GetTower(TowerType.BananaFarm, top, mid, bot);
        } else if (id.Contains("spike") || id.Contains("factory")) {
            return Game.instance.model.GetTower(TowerType.SpikeFactory, top, mid, bot);
        } else if (id.Contains("village")) {
            return Game.instance.model.GetTower(TowerType.MonkeyVillage, top, mid, bot);
        } else if (id.Contains("engineer")) {
            return Game.instance.model.GetTower(TowerType.EngineerMonkey, top, mid, bot);
        } else if (id.Contains("beast") || id.Contains("handler")) {
            return Game.instance.model.GetTower(TowerType.BeastHandler, top, mid, bot);
        } else {
            return Game.instance.model.GetTower(id, top, mid, bot);
        }
    }
}
