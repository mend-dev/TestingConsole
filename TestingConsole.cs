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
            }
        } else if (command == "tower") {
            string subCommand = values[1];

            if (subCommand == "place") {
                TowerModel tower = GetTower(values[2]);
                int xPos = int.Parse(values[3]);
                int yPos = int.Parse(values[4]);

                game.bridge.CreateTowerAt(new Vector2(xPos, yPos), tower, new Il2CppAssets.Scripts.ObjectId(), false, null);
            }

        } else if (command == "cash") {
            string subCommand = values[1];
            int amount = int.Parse(values[2]);

            if (subCommand == "add") {
                game.SetCash(game.GetCash() + amount);
            } else if (subCommand == "remove") {
                game.SetCash(game.GetCash() - amount);
            } else if (subCommand == "set") {
                game.SetCash(amount);
            }

        } else if (command == "lives") {
            string subCommand = values[1];
            int amount = int.Parse(values[2]);

            if (subCommand == "add") {
                game.SetHealth(game.GetHealth() + amount);
            } else if (subCommand == "remove") {
                game.SetHealth(game.GetHealth() - amount);
            } else if (subCommand == "set") {
                game.SetHealth(amount);
            }

        } else if (command == "round") {
            string subCommand = values[1];
            int amount = int.Parse(values[2]);

            if (subCommand == "add") {
                game.SetRound(game.bridge.GetCurrentRound() + amount);
            } else if (subCommand == "remove") {
                game.SetRound(game.bridge.GetCurrentRound() - amount);
            } else if (subCommand == "set") {
                game.SetRound(amount);
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

    private TowerModel GetTower(string towerName) {
        return towerName switch {
            "dart" => Game.instance.model.GetTower(TowerType.DartMonkey, 0, 0, 0),
            "boomer" => Game.instance.model.GetTower(TowerType.BoomerangMonkey, 0, 0, 0),
            "bomb" => Game.instance.model.GetTower(TowerType.BombShooter, 0, 0, 0),
            "tack" => Game.instance.model.GetTower(TowerType.TackShooter, 0, 0, 0),
            "ice" => Game.instance.model.GetTower(TowerType.IceMonkey, 0, 0, 0),
            "glue" => Game.instance.model.GetTower(TowerType.GlueGunner, 0, 0, 0),
            "sniper" => Game.instance.model.GetTower(TowerType.SniperMonkey, 0, 0, 0),
            "sub" => Game.instance.model.GetTower(TowerType.MonkeySub, 0, 0, 0),
            "buccaneer" => Game.instance.model.GetTower(TowerType.MonkeyBuccaneer, 0, 0, 0),
            "ace" => Game.instance.model.GetTower(TowerType.MonkeyAce, 0, 0, 0),
            "heli" => Game.instance.model.GetTower(TowerType.HeliPilot, 0, 0, 0),
            "mortar" => Game.instance.model.GetTower(TowerType.MortarMonkey, 0, 0, 0),
            "dartling" => Game.instance.model.GetTower(TowerType.DartlingGunner, 0, 0, 0),
            "wizard" => Game.instance.model.GetTower(TowerType.WizardMonkey, 0, 0, 0),
            "super" => Game.instance.model.GetTower(TowerType.SuperMonkey, 0, 0, 0),
            "ninja" => Game.instance.model.GetTower(TowerType.NinjaMonkey, 0, 0, 0),
            "alchemist" => Game.instance.model.GetTower(TowerType.Alchemist, 0, 0, 0),
            "druid" => Game.instance.model.GetTower(TowerType.Druid, 0, 0, 0),
            "farm" => Game.instance.model.GetTower(TowerType.BananaFarm, 0, 0, 0),
            "spike" => Game.instance.model.GetTower(TowerType.SpikeFactory, 0, 0, 0),
            "village" => Game.instance.model.GetTower(TowerType.MonkeyVillage, 0, 0, 0),
            "engineer" => Game.instance.model.GetTower(TowerType.EngineerMonkey, 0, 0, 0),
            "beast" => Game.instance.model.GetTower(TowerType.BeastHandler, 0, 0, 0),
        };
    }
}
