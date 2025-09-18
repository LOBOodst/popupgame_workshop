using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MailBox : MonoBehaviour
{
    [SerializeField] enum typeOfMails { DrCat, Mamie, Dino, CapZ, Stag, Saxo};
    [SerializeField] typeOfMails mailTo;

    [SerializeField] Button buttonDrCat;
    [SerializeField] Button buttonMamie;
    [SerializeField] Button buttonDino;
    [SerializeField] Button buttonCapZ;
    [SerializeField] Button buttonStag;
    [SerializeField] Button buttonSaxo;

    [SerializeField] private int index;
    [SerializeField] private TextMeshProUGUI objectText;
    [SerializeField] private TextMeshProUGUI mainText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switch (mailTo)
        {
                case typeOfMails.DrCat: 
                    objectText.text = "Object : Donnez pour les sauver ";
                    mainText.text = "Bonjour,\r\nNous avons bien prit en compte votre demande. N�anmoins ne nous pouvons pas nous permettre de faire un don de 1 800 000 euros afin de sauver les chatons en Chatimanie. \r\n\r\nCordialement. \r\nPierrot Mann";
                    index = 0;
                break;

                case typeOfMails.Mamie:
                    objectText.text = "Object : Ca fait longtemps";
                    mainText.text = "Bonjour <3\r\nJe suis d�sol�e je travail beaucoup en ce moment, mais je te promet de t'appeler ce week-end. \r\nPs : Si tu pouvais utiliser ma bo�te mail personnel la prochaine fois. \r\n\r\nBisous mamie\r\nPierrot";
                    index = 1;
                break;
                case typeOfMails.Dino:
                    objectText.text = "Object : Qui a br�l� mon bureau?";
                    mainText.text = "Bonjour.\r\nCalmez-vous. Nous allons chercher qui est le malfaiteur ayant saccag� votre lieu de travail. Je suis d'accord avec vous, l'harc�lement que vous subissez � cause de votre passion pour la Pr�histoire est un scandal !\r\nN�anmoins, je tiens � vous demander de communiquer plus poliment la prochaine fois...\r\n\r\nCordialement,\r\nPierrot Mann";
                    index = 2;
                break;
                case typeOfMails.CapZ:
                    objectText.text = "Object : Team building sur le th�me \"pirate\"";
                    mainText.text = "Bonjour Monsieur,\r\nConcernant votre proposition d'activit�, je me dois de vous dire que nous avons d�j� utilis� ce th�me l'ann�e pass�e et l'ann�e d'encore avant. J'ai bien conscience que vous �tes un grand fan des loups marins et des Kraken, mais pourrions-nous pas changer pour une fois ?\r\n\r\nCordialement,\r\nPierrot Mann";
                    index = 3;
                break;
                case typeOfMails.Stag:
                    objectText.text = "Object : Je trouve pas les toilettes";
                    mainText.text = "Bonjour,\r\nIl est vrai que le b�timent est assez grand, poss�de de nombreux couloirs et bureaux. Votre demande est donc totalement compr�hensible, ne vous en faites pas.\r\n\r\nPour trouver le chemin qui r�alisera vos envies les plus pressantes, prenez le deuxi�me couloir � gauche en partant de l'entr�e principale.\r\nEnsuite, vous devez passer la statue de dinosaure et vous trouverez ce que vous cherchez.\r\n\r\nCordialement,\r\nPierrot Mann";
                    index = 4;
                break;
                case typeOfMails.Saxo:
                    objectText.text = "Object : Publicit� et droit d'auteur";
                    mainText.text = "Bonjour,\r\nNous avons bien re�u votre requ�te, et comprenons votre m�contentement suite � la piste audio utiliser pour la publicit� de notre entreprise.\r\nJe vous prie de contacter le service d�di� par t�l�phone au 0262 8989898989 ou par mail : servicejuridique@mail.com\r\n\r\nCordialement.\r\nPierrot Mann";
                    index = 5;
                break;
        };

        buttonDrCat.onClick.AddListener(DrCatClick);
        buttonMamie.onClick.AddListener(MamieClick);
        buttonDino.onClick.AddListener(DinoClick);
        buttonCapZ.onClick.AddListener(CapZClick);
        buttonStag.onClick.AddListener(StagClick);
        buttonSaxo.onClick.AddListener(SaxoClick);
    }


    void DrCatClick()
    {
        if (index == 0)
        {
            Destroy(gameObject);
        }
        else Debug.Log("false mail");
    }

    void MamieClick()
    {
        if (index == 1)
        {
            Destroy(gameObject);
        }
        else Debug.Log("false mail");
    }
    void DinoClick()
    {
        if (index == 2)
        {
            Destroy(gameObject);
        }
        else Debug.Log("false mail");
    }
    void CapZClick()
    {
        if (index == 3)
        {
            Destroy(gameObject);
        }
        else Debug.Log("false mail");
    }
    void StagClick()
    {
        if (index == 4)
        {
            Destroy(gameObject);
        }
        else Debug.Log("false mail");
    }
    void SaxoClick()
    {
        if (index == 5)
        {
            Destroy(gameObject);
        }
        else Debug.Log("false mail");
    }
}
