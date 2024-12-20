using System.Collections;
using UnityEngine;
using Thirdweb;
using Thirdweb.Unity;
using TMPro;
using System.Numerics;
using System;
using UnityEngine.SceneManagement;

public class BlockchainManager : MonoBehaviour
{
    private const string playerTokenString = "PlayerToken";
    public TMP_Text logText;

    public string Address { get; private set; }
    public static BigInteger ChainId = 204;

    public UnityEngine.UI.Button playButton;
    public UnityEngine.UI.Button getBalanceButton;

    public TMP_Text playButtonText;

    string customSmartContractAddress = "0x6a2c574AF252D7205fdd601851F0ab27829E10D8";
    string abiString = "[{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"newScore\",\"type\":\"uint256\"}],\"name\":\"ScoreIncreased\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"addOnePoint\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"getPlayerScore\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";

    int tokenAmount = 1;

    string notEnoughToken = " BNB";

    private void Start()
    {
        if (PlayerPrefs.HasKey(playerTokenString))
        {
            int currentTokens = PlayerPrefs.GetInt(playerTokenString);
            if (currentTokens > 0)
            {
                playButtonText.text = "Play";
            }
            else
            {
                playButtonText.text = "Get Pass";
            }
        }
        else
        {
            playButtonText.text = "Get Pass";
        }
    }

    public void SwitchToMainMenuScene()
    {
        if (PlayerPrefs.HasKey(playerTokenString))
        {
            int currentTokens = PlayerPrefs.GetInt(playerTokenString);
            if (currentTokens > 0)
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                GetTokens();
            }
        }
        else
        {
            GetTokens();
        }
    }

    private void AddPlayerToken()
    {
        int currentTokens = PlayerPrefs.GetInt(playerTokenString, 0);
        currentTokens++;
        PlayerPrefs.SetInt(playerTokenString, currentTokens);
        PlayerPrefs.Save();
        currentTokens = PlayerPrefs.GetInt(playerTokenString, 0);
        Debug.Log("PlayerToken updated to: " + currentTokens);
        playButtonText.text = "Play";
    }

    private void HideAllButtons()
    {
        playButton.interactable = false;
        getBalanceButton.interactable = false;
    }

    private void ShowAllButtons()
    {
        playButton.interactable = true;
        getBalanceButton.interactable = true;
    }

    private void UpdateStatus(string messageShow)
    {
        logText.text = messageShow;
    }

    private void BoughtSuccessFully()
    {
        AddPlayerToken();
        UpdateStatus("Got 1 Tokens");
    }
    IEnumerator WaitAndExecute()
    {
        Debug.Log("Coroutine started, waiting for 3 seconds...");
        yield return new WaitForSeconds(3f);
        Debug.Log("3 seconds have passed!");
        BoughtSuccessFully();
        ShowAllButtons();
    }

    private async void Claim1Token()
    {
        var wallet = ThirdwebManager.Instance.GetActiveWallet();
        var contract = await ThirdwebManager.Instance.GetContract(
           customSmartContractAddress,
           ChainId,
           abiString
       );
        var address = await wallet.GetAddress();
        await ThirdwebContract.Write(wallet, contract, "addOnePoint", 0, address);

        var result = ThirdwebContract.Read<int>(contract, "getPlayerScore", address);
        Debug.Log("result: " + result);
    }

    public async void GetTokens()
    {
        HideAllButtons();
        UpdateStatus("Getting 1 Token...");
        var wallet = ThirdwebManager.Instance.GetActiveWallet();
        var balance = await wallet.GetBalance(chainId: ChainId);
        var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
        Debug.Log("balanceEth1: " + balanceEth);
        if (float.Parse(balanceEth) <= 0f)
        {
            UpdateStatus("Not Enough" + notEnoughToken);
            ShowAllButtons();
            return;
        }
        StartCoroutine(WaitAndExecute());
        try
        {
            Claim1Token();
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred during the transfer: {ex.Message}");
        }
    }
}
