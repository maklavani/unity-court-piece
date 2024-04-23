using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
	public GameManager gameManager;
    public GameObject underCardsLayout;
	public GameObject cardsLayout;
	public GameObject cardPrefab;
	public GameObject setRule;

    [HideInInspector]
    public List<CardData> spriteList;
    [HideInInspector]
    public List<GameObject> cards = new List<GameObject>();
    [HideInInspector]
    public List<Animation> animations = new List<Animation>();

    [HideInInspector]
	public bool animationsEnd = false;
	[HideInInspector]
	public bool destroyEnd = true;
	[HideInInspector]
	public bool animationGovernoverEnd = false;
	[HideInInspector]
	public bool animationRuleStart = false;
	[HideInInspector]
	public bool cardClickAnimation = false;
	[HideInInspector]
	public bool cardClickStep = false;
	[HideInInspector]
	public Card cardClicked = new Card ();
	[HideInInspector]
	public MoveData lastCardClick = new MoveData ();

	private float cardWidth = 0f;
	private float cardHeight = 0f;
	private float paddingWidth = 20f;
	private float paddingHeight = 160f;
	private float rotation = 30f;

    void Update()
    {
        if (cards.Count > 0)
        {
            SetSize();
            SetPosition();
        }

        if (animations.Count > 0)
		{
            for (var i = 0; i < animations.Count; i++)
            {
                if (animations[i].status)
                {
					animationsEnd = false;

					Animation animation = animations[i];
					var cp = animation.go.GetComponent<CardProperty>();

                    // Move Animation
                    animation.go.transform.localPosition = Vector3.MoveTowards(animation.go.transform.localPosition, animation.targetPosition, animation.speed);

					// Rotate Animation
                    if (animation.go.transform.rotation.z != animation.rotate)
						animation.go.transform.rotation = Quaternion.RotateTowards(animation.go.transform.rotation, Quaternion.Euler(0,0,animation.rotate), animation.speed * 2);

					// Scale Animation
					if (!V3Equal (cp.scale, animation.scale)) {
						var rt = animation.go.GetComponent<RectTransform> ();
						rt.sizeDelta = Vector2.MoveTowards (new Vector2(rt.rect.width , rt.rect.height) , new Vector2(cardWidth * animation.scale.x , cardHeight * animation.scale.y) , animation.speed);
					}

                    // Residane be maghsad
                    if (V3Equal(animation.go.transform.localPosition, animation.targetPosition))
					{
						cp.position = animation.targetPosition;
						cp.rotate = animation.rotate;
						cp.scale = animation.scale;
                        cp.animate = false;

                        // Next Enabled
                        if (i + 1 < animations.Count)
                        {
                            animations[i + 1].status = true;
                            animations[i + 1].go.transform.SetSiblingIndex(52);

							// Enable Sound
							if(animations[i + 1].sound)
								gameManager.soundsManager.EnableSound ("Move");
                        }
                        else
                            animationsEnd = true;

                        // Remove Animation
						if (cp.used)
							animation.go.transform.Find ("Face").gameObject.SetActive (true);
							
                        animations.RemoveAt(i);
                    }
                }
            }
        }
        else if (animationsEnd)
        {
            // Destroy After Set Governor Animation
			if (!animationGovernoverEnd && gameManager.rule == 0 && cards.Count > 0) {
				// Destroy Card Animation
				if (gameManager.scoreTeamA == 0 && gameManager.scoreTeamB == 0)
					StartCoroutine (DestroyCards (3f, 1f));

				animationGovernoverEnd = true;

				// Set Crown Icon
				gameManager.SetCrownIcon();
			} else if (animationGovernoverEnd && gameManager.rule == 0 && cards.Count > 0) {
				if (gameManager.stepSetGovernor == 0) {
					gameManager.stepSetGovernor = 1;

					if (gameManager.governor == gameManager.myTurn) {
						setRule.SetActive (true);
						gameManager.gameControl.translateLanguage = true;
					}

					// Disable Get Movement
					Debug.Log("Show Set Rule");
					gameManager.disableGetMovement = false;
				}
			} else if (gameManager.rule != 0 && gameManager.stepSetGovernor == 1) {
				Debug.Log ("Animation For Set Rule");
				gameManager.stepSetGovernor = 2;
				StartCoroutine (animationForSetRule (gameManager.governor, 9, 12));
			} else if (gameManager.rule != 0 && gameManager.stepSetGovernor == 2) {
				gameManager.stepSetGovernor = 3;
				StartCoroutine (animationForSort ());

				// Disable Get Movement
				Debug.Log("Animation For Sort");
				gameManager.disableGetMovement = false;
			}

			// Card Click
			if(cardClickAnimation){
				cardClickAnimation = false;

				gameManager.cardType = (int)lastCardClick.type;
				gameManager.cardNumber = lastCardClick.number;
				gameManager.requestManager.EnableRequest ("Send Data");
			}

            animationsEnd = false;
        }
        else if (animationGovernoverEnd && !animationRuleStart && gameManager.rule == 0 && destroyEnd)
        {
            initCards(gameManager.cards , false);

			// Set Governor Animate
			if(gameManager.maxDelay > 9)
				StartCoroutine (animationForSetRule (gameManager.governor , 0 , 4));
			else
				StartCoroutine (animationForSetRule (gameManager.governor , 0 , 4 , false));
            animationRuleStart = true;
        }
    }

    // Initailize Cards
    public void initCards(List<Card> list , bool showFace = true)
	{
        // Destroy
        if (cards.Count > 0)
            StartCoroutine(DestroyCards(0f));

        if (list.Count > 0)
        {
            for (var i = 0; i < list.Count; i++)
            {
                Card listItem = list[i];
                // Init Card
                GameObject go = Instantiate(cardPrefab, cardsLayout.transform);
                // Init Card Property
				var cp = go.GetComponent<CardProperty>();
                cp.card.number = listItem.number;
				cp.card.type = listItem.type;
				cp.turn = (int)(i / 13) + 1;
				cp.index = i - (cp.turn - 1) * 13;
                cp.animate = false;
                cp.canClick = false;
				cp.myCard = false;
				cp.used = false;
				cp.position = Vector3.zero;
				cp.rotate = 0f;
				cp.scale = new Vector3 (1f, 1f);

                // Set Name
                go.name = "Card_" + cp.card.type + "_" + cp.card.number;
                cards.Add(go);

                // Set Image
                if (!showFace)
                    go.transform.Find("Face").gameObject.SetActive(true);

                var image = go.GetComponent<Image>();
                image.sprite = spriteList[((int)cp.card.type - 1) * 13 + cp.card.number - 1].image;

                // Set Button
                var btn = go.GetComponent<Button>();
                btn.interactable = cp.canClick;

				btn.onClick.AddListener (delegate {
					CardClick (btn);
				});
            }

            // Set Size
            SetSize();

            // Set Siblings
            for (var i = 0; i < cards.Count; i++)
                cards[i].transform.SetSiblingIndex(52 - i);
        }
    }

    public IEnumerator DestroyCards(float delay = 0f , float endDelay = 0f)
    {
        destroyEnd = false;
        List<GameObject> cardsDestry = new List<GameObject>(cards);
        cards = new List<GameObject>();
        yield return new WaitForSeconds(delay);
   
        if (cardsDestry.Count > 0)
            for (var i = 0; i < cardsDestry.Count; i++)
            {
                if(delay != 0f)
                    yield return new WaitForSeconds(Time.deltaTime);

                Destroy(cardsDestry[i]);
            }

		yield return new WaitForSeconds(endDelay);
		destroyEnd = true;
    }

    // Set Size
    public void SetSize()
    {
        var cardsLayoutRT = cardsLayout.GetComponent<RectTransform>();

        float aspectRatio = cardsLayoutRT.rect.width / cardsLayoutRT.rect.height;
        float cardAspectRatio = 3f / 5f;
        
        if (aspectRatio < 1f) {
            // Portrait
            cardWidth = cardsLayoutRT.rect.width / 5f;
            cardHeight = cardWidth / cardAspectRatio;
        } else {
            // Landscape
            cardWidth = cardsLayoutRT.rect.width / 8f;
            cardHeight = cardWidth / cardAspectRatio;
        }
   
        if (cards.Count > 0)
            foreach (var cardItem in cards)
			{
				var cp = cardItem.GetComponent<CardProperty>();
		
				if (!cp.animate) {
					var rt = cardItem.GetComponent<RectTransform>();
					rt.sizeDelta = new Vector2 (cardWidth * cp.scale.x, cardHeight * cp.scale.y);
				}
            }
    }

	// Set Position
	public void SetPosition()
	{
		if (cards.Count > 0)
			foreach (var cardItem in cards)
			{
				var cp = cardItem.GetComponent<CardProperty>();

				if (!cp.animate)
				{
					var rt = cardItem.GetComponent<RectTransform>();
					rt.localPosition = cp.position;
					cardItem.transform.rotation = Quaternion.Euler (0f, 0f, cp.rotate);
				}
			}
	}

	// Set Position Uses Card
	public void SetPositionUsesCard()
	{
		if (gameManager.tableGame.Count > 1) {
			int teamAWin = 0;
			int teamBWin = 0;

			foreach (var table in gameManager.tableGame)
				if (table.win != 0) {
					if (table.win == 1 || table.win == 3)
						teamAWin++;
					else
						teamBWin++;

					foreach (var move in table.moves) {
						GameObject go = GameObject.Find ("Card_" + move.type + "_" + move.number);
						var cp = go.GetComponent<CardProperty> ();
						string turnPos = GetTurnPosition (table.win);

						cp.used = true;
						cp.position = GetCardPositionUsed (turnPos, table.win == 1 || table.win == 3 ? teamAWin : teamBWin);
						cp.rotate = GetCardRotationUsed (turnPos);
						cp.scale = new Vector2 (0.3f, 0.3f);

						go.transform.Find ("Face").gameObject.SetActive (true);
					}
				}
		}
	}

	// Set Position Center Card
	public IEnumerator SetPositionCenterCard()
	{
		yield return new WaitForSeconds (0.5f);

		// Set Center Card
		if (gameManager.lastMovement.moves.Count > 0 && gameManager.lastMovement.win == 0) {
			foreach (var move in gameManager.lastMovement.moves) {
				GameObject go = GameObject.Find ("Card_" + move.type + "_" + move.number);
				var cp = go.GetComponent<CardProperty> ();
				var btn = go.GetComponent<Button> ();

				cp.animate = true;
				cp.transform.Find ("Face").gameObject.SetActive (false);
				ColorBlock cb = btn.colors;
				cb.disabledColor = new Color (1f, 1f, 1f, 1f);
				btn.colors = cb;

				gameManager.numberCardInCenter++;
				CreateAniamtionForCardClick (go, GetTurnPosition (cp.turn));
			}

			// Enable Sound
			if(animations[0].sound)
				gameManager.soundsManager.EnableSound ("Move");
		}
	}

	// Show Last Movement
	public void ShowLastMovement()
	{
		// Set Center Card
		if (gameManager.lastMovement.moves.Count > gameManager.numberCardInCenter) {
			MoveData move = gameManager.lastMovement.moves [gameManager.lastMovement.moves.Count - 1];
			GameObject go = GameObject.Find ("Card_" + move.type + "_" + move.number);
			var cp = go.GetComponent<CardProperty> ();
			var btn = go.GetComponent<Button> ();

			cp.animate = true;
			cp.transform.Find ("Face").gameObject.SetActive (false);
			ColorBlock cb = btn.colors;
			cb.disabledColor = new Color(1f , 1f , 1f , 1f);
			btn.colors = cb;

			gameManager.numberCardInCenter++;
			CreateAniamtionForCardClick (go, GetTurnPosition (cp.turn));

			// Enable Sound
			if(animations[0].sound)
				gameManager.soundsManager.EnableSound ("Move");
		}
	}

	// Card Selection
	public void CardSelection()
	{
		List<MoveData> posibleMove = GetPosibleMove (gameManager.myTurn);

		if (posibleMove.Count > 0)
			foreach(var pm in posibleMove){
				GameObject go = GameObject.Find ("Card_" + pm.type + "_" + pm.number);
				Button btn = go.GetComponent<Button> ();
				btn.interactable = true;
			}
	}

	// Card Click
	public void CardClick(Button btn)
	{
		GameObject go = btn.gameObject;
		var cp = go.GetComponent<CardProperty> ();

		if (cardClickStep && cp.card.number == cardClicked.number && cp.card.type == cardClicked.type) {
			cp.animate = true;

			// Set Color
			ColorBlock cb = btn.colors;
			cb.disabledColor = new Color (1f, 1f, 1f, 1f);
			btn.colors = cb;

			// Create Animarion
			gameManager.numberCardInCenter++;
			CreateAniamtionForCardClick (go, GetTurnPosition (cp.turn));

			// Enable Sound
			if (animations [0].sound)
				gameManager.soundsManager.EnableSound ("Move");

			// Set Information Click
			lastCardClick.type = cp.card.type;
			lastCardClick.number = cp.card.number;
			lastCardClick.turn = cp.turn;
			cardClickStep = false;
			cardClickAnimation = true;

			DisableAllSelection ();
		} else {
			DisableCardClick ();

			cardClickStep = true;
			cardClicked.type = cp.card.type;
			cardClicked.number = cp.card.number;

			cp.position = new Vector2 (cp.position.x , cp.position.y + 5f);
		}
	}

	// Disable Card Click
	public void DisableCardClick()
	{
		if (cardClickStep) {
			GameObject go = GameObject.Find ("Card_" + cardClicked.type + "_" + cardClicked.number);
			var cp = go.GetComponent<CardProperty> ();

			cp.position = new Vector2 (cp.position.x , cp.position.y - 5f);
			cardClickStep = false;
		}
	}

	// Create Aniamtion For Card Click
	public void CreateAniamtionForCardClick(GameObject go , string turnPos) {
		go.transform.SetSiblingIndex (52);

		Animation animation = new Animation ();
		animation.status = true;
		animation.go = go;
		animation.targetPosition = GetCardPositionCenter (turnPos);
		animation.rotate = Random.Range (-15f, 15f);
		animation.scale = new Vector2(1.0f , 1.0f);
		animation.speed = Time.deltaTime * 400;
		animation.sound = true;
		animations.Add (animation);
	}

	// Disable All Selection
	public void DisableAllSelection()
	{
		if (cards.Count > 0)
			foreach (var cardItem in cards) {
				var btn = cardItem.GetComponent<Button> ();
				btn.interactable = false;
			}
	}

	// Get Posible Move
	public List<MoveData> GetPosibleMove(int turn)
	{
		List<MoveData> posibleCards = new List<MoveData> ();

		if (cards.Count > 0) {
			foreach (var cardItem in cards) {
				var cp = cardItem.GetComponent<CardProperty> ();

				if (cp.turn == turn) {
					MoveData move = new MoveData ();
					move.type = cp.card.type;
					move.number = cp.card.number;
					move.turn = cp.turn;
					posibleCards.Add (move);
				}
			}
		}
				
		if (posibleCards.Count > 0) {
			// Remove Uses Card
			if (gameManager.tableGame.Count > 0)
				foreach (var table in gameManager.tableGame)
					if (table.moves.Count > 0)
						foreach (var move in table.moves)
							if (move.turn == turn)
								for (var i = 0; i < posibleCards.Count; i++)
									if (posibleCards [i].type == move.type && posibleCards [i].number == move.number) {
										posibleCards.RemoveAt (i);
										break;
									}

			// Remove Setted Ruled
			if (gameManager.lastMovement.moves.Count > 0 && gameManager.lastMovement.win == 0) {
				MoveData firstMove = gameManager.lastMovement.moves [0];
				bool haveAnyCards = HaveCardsWithMoveData (posibleCards , firstMove);

				if (haveAnyCards) {
					List<MoveData> posibleCardsB = new List<MoveData> ();
	
					for (var i = 0; i < posibleCards.Count; i++)
						if (posibleCards [i].type == firstMove.type)
							posibleCardsB.Add (posibleCards [i]);

					posibleCards = posibleCardsB;
				}
			}
		}

		return posibleCards;
	}

	// Have Cards With Move Data
	public bool HaveCardsWithMoveData(List<MoveData> moves , MoveData moveForFind)
	{
		bool output = false;

		foreach (var move in moves)
			if (move.type == moveForFind.type) {
				output = true;
				break;
			}

		return output;
	}

    // Animation For Set Governor
	public IEnumerator animationForSetGovernor()
	{
		animationsEnd = false;

		yield return new WaitForSeconds (0.5f);

        for (var i = 0; i < cards.Count; i++)
        {
            GameObject cardItem = cards[i];
            cardItem.transform.SetSiblingIndex(52 - i);

            var cp = cardItem.GetComponent<CardProperty>();
            cp.animate = true;

            cp.turn = i % 4 + 1;
            string turnPos = GetTurnPosition(cp.turn);

            Animation animation = new Animation();
            animation.status = false;
            animation.go = cardItem;
			animation.targetPosition = GetTargetSetGovernorPosition(turnPos);
			// animation.rotate = Random.Range(0f , 360f);
			animation.rotate = 0f;
			animation.scale = new Vector3 (0.8f, 0.8f);
			animation.speed = Time.deltaTime * 1000;
			animation.sound = true;
            animations.Add(animation);

            // Exit - Found AS
            if (cp.card.number == 1)
                break;
        }

        // Start First Animation
		animations[0].status = true;

		// Enable Sound
		if (animations [0].sound)
			gameManager.soundsManager.EnableSound ("Move");
    }

	// Animation For Set Governor
	public IEnumerator animationForSetRule(int firstTurn , int fromIndex , int toIndex , bool animate = true)
	{
		animationsEnd = false;

		yield return new WaitForSeconds (0f);

		for (var i = 0; i < cards.Count; i++) {
			int j = (i + (firstTurn - 1) * 13) % 52;
			GameObject cardItem = cards [j];

			var cp = cardItem.GetComponent<CardProperty> ();

			if (gameManager.tv || j >= (gameManager.myTurn - 1) * 13 && j < gameManager.myTurn * 13)
				cp.myCard = true;

			string turnPos = GetTurnPosition (cp.turn);

			if ((fromIndex == 5 || cp.index >= fromIndex) && cp.index <= toIndex) {
				int indexShow = cp.index;

				if (fromIndex == 0 && toIndex == 4)
					indexShow += 4;
					// indexShow *= 3;

				if (cp.myCard && !cp.used)
					cardItem.transform.Find ("Face").gameObject.SetActive (false);

				cardItem.transform.SetSiblingIndex (52 - i);

				if (!cp.used) {
					if (animate) {
						cp.animate = true;

						Animation animation = new Animation ();
						animation.status = false;
						animation.go = cardItem;
						animation.targetPosition = GetCardPositionWithIndex (indexShow, turnPos);
						animation.rotate = GetCardRotationWithIndex (indexShow, turnPos);
						animation.scale = GetCardSacle (turnPos);
						animation.speed = Time.deltaTime * 800;

						if ((cp.index < 5 && fromIndex == 0) || cp.index >= 5)
							animation.sound = true;
						else
							animation.sound = false;

						animations.Add (animation);
					} else {
						cardItem.transform.SetSiblingIndex (52 + i);

						cp.position = GetCardPositionWithIndex (indexShow, turnPos);
						cp.rotate = GetCardRotationWithIndex (indexShow, turnPos);
						cp.scale = GetCardSacle (turnPos);
					}
				}
			}
		}

		// Start First Animation
		if (animate && animations.Count > 0) {
			animations [0].status = true;

			// Enable Sound
			if (animations [0].sound)
				gameManager.soundsManager.EnableSound ("Move");
		}
	}

	// Animation For Set Governor
	public IEnumerator animationForSort(bool animate = true)
	{
		animationsEnd = false;

		yield return new WaitForSeconds (0f);
		List<int> indexes = new List<int> ();
		List<int> values = new List<int> ();

		int start = !gameManager.tv ? (gameManager.myTurn - 1) * 13 : 0;
		int end = !gameManager.tv ? gameManager.myTurn * 13 : 52;

		for (var i = start; i < end; i++) {
			GameObject cardItem = cards [i];
			var cp = cardItem.GetComponent<CardProperty> ();
			indexes.Add (i);

			if(cp.card.number != 1)
				values.Add ((int)cp.card.type * 13 + cp.card.number - 1);
			else
				values.Add ((int)cp.card.type * 13 + 13);
		}

		// Bubble Sort
		int loop = !gameManager.tv ? 1 : 4;

		for (var k = 0; k < loop; k++) {
			start = !gameManager.tv ? 0 : k * 13;
			end = !gameManager.tv ? 13 : (k + 1) * 13;

			for (var i = start; i < end; i++)
				for (var j = start; j < end; j++)
					if (values [i] < values [j]) {
						int value = values [i];
						values [i] = values [j];
						values [j] = value;

						int index = indexes [i];
						indexes [i] = indexes [j];
						indexes [j] = index;
					}

			for (var i = start; i < end; i++) {
				GameObject cardItem = cards [indexes [i]];
				var cp = cardItem.GetComponent<CardProperty> ();
				cp.index = i % 13;
				int indexShow = cp.index;
				cardItem.transform.SetSiblingIndex (52 + i);
				string turnPos = GetTurnPosition (!gameManager.tv ? gameManager.myTurn : k + 1);

				if (!cp.used) {
					if (animate) {
						cp.animate = true;
						Animation animation = new Animation ();
						animation.status = false;
						animation.go = cardItem;
						animation.targetPosition = GetCardPositionWithIndex (indexShow, turnPos);
						animation.rotate = GetCardRotationWithIndex (indexShow, turnPos);
						animation.scale = GetCardSacle (turnPos);
						animation.speed = Time.deltaTime * 1200;
						animation.sound = false;
						animations.Add (animation);
					} else {
						cp.position = GetCardPositionWithIndex (indexShow, turnPos);
						cp.rotate = GetCardRotationWithIndex (indexShow, turnPos);
						cp.scale = GetCardSacle (turnPos);
					}
				}
			}
		}

		// Start First Animation
		if (animate && animations.Count > 0)
			animations [0].status = true;
	}

	// Animation For Center
	public IEnumerator animationForCenter()
	{
		animationsEnd = false;

		yield return new WaitForSeconds (1f);
		int teamAWin = 0;
		int teamBWin = 0;

		if (gameManager.tableGame.Count > 1) {
			for (var i = 0; i < gameManager.tableGame.Count; i++) {
				TableGameData table = gameManager.tableGame [i];

				if (table.win != 0) {
					if (table.win == 1 || table.win == 3)
						teamAWin++;
					else
						teamBWin++;
				}
			}
		}

		if (gameManager.lastMovement.moves.Count > 0) {
			foreach (var move in gameManager.lastMovement.moves) {
				GameObject go = GameObject.Find ("Card_" + move.type + "_" + move.number);
				var cp = go.GetComponent<CardProperty> ();

				int indexShow = cp.index;
				string turnPos = GetTurnPosition (gameManager.lastMovement.win);

				cp.animate = true;
				cp.used = true;

				Animation animation = new Animation ();
				animation.status = true;
				animation.go = go;
				animation.targetPosition = GetCardPositionUsed (turnPos, gameManager.lastMovement.win == 1 || gameManager.lastMovement.win == 3 ? teamAWin : teamBWin);
				animation.rotate = GetCardRotationUsed (turnPos);
				animation.scale = new Vector2 (0.3f, 0.3f);
				animation.speed = Time.deltaTime * 400;
				animation.sound = false;

				animations.Add (animation);
			}
		}

		// Enable Sound
		if (animations.Count > 0 && animations [0].sound)
			gameManager.soundsManager.EnableSound ("Move");
	}

    // Get Target Set Governor Position
    public Vector2 GetTargetSetGovernorPosition(string turnPos)
	{
		var cardsLayoutRT = cardsLayout.GetComponent<RectTransform> ();
		Vector2 pos = new Vector2 ();

		if (turnPos == "Bottom")
			pos = new Vector2 (0, -0.5f * (cardsLayoutRT.rect.height - cardHeight) + cardHeight / 2.5f);
		else if (turnPos == "Right")
			pos = new Vector2 (0.5f * (cardsLayoutRT.rect.width - cardWidth) - cardWidth / 1.5f, 0);
		else if (turnPos == "Top")
			pos = new Vector2 (0, 0.5f * (cardsLayoutRT.rect.height - cardHeight) - cardHeight / 2.5f);
		else
			pos = new Vector2 (-0.5f * (cardsLayoutRT.rect.width - cardWidth) + cardWidth / 1.5f, 0);

		return pos;
	}

	// Get Card Position With Index
	public Vector2 GetCardPositionWithIndex(int index , string turnPos)
	{
		var cardsLayoutRT = cardsLayout.GetComponent<RectTransform>();
		int finalIndex = index - 6;

		float widthColumn = (cardsLayoutRT.rect.width - paddingWidth - cardWidth) / 13;
		float heightColumn = (cardsLayoutRT.rect.height - paddingHeight - cardWidth) / 13; // card Height Nist

		float rotateColumn = Mathf.PI / (13 - 1);

		Vector2 pos = new Vector2();

		if (turnPos == "Bottom") {
			pos.x = finalIndex * widthColumn;
			pos.y = -0.5f * (cardsLayoutRT.rect.height - cardHeight) + cardHeight / 10;
			pos.y += Mathf.Sin ((12 - index) * rotateColumn) * (cardHeight / 10);
		} else if (turnPos == "Right") {
			pos.x = 0.5f * (cardsLayoutRT.rect.width - cardWidth) - cardWidth / 3;
			pos.x -= Mathf.Sin (index * rotateColumn) * (cardHeight / 10);
			if(!gameManager.tv)
				pos.y = finalIndex * heightColumn;
			else
				pos.y = 1.5f * finalIndex * heightColumn;
		} else if (turnPos == "Top") {
			if(!gameManager.tv)
				pos.x = -1 * (finalIndex * widthColumn / 2);
			else
				pos.x = -1 * finalIndex * widthColumn;
			pos.y = 0.5f * (cardsLayoutRT.rect.height - cardHeight) - cardHeight / 5;
			pos.y -= Mathf.Sin (index * rotateColumn) * (cardHeight / 10);
		} else {
			pos.x = -0.5f * (cardsLayoutRT.rect.width - cardWidth) + cardWidth / 3;
			pos.x += Mathf.Sin ((12 - index) * rotateColumn) * (cardHeight / 10);
			if(!gameManager.tv)
				pos.y = -1 * (finalIndex * heightColumn);
			else
				pos.y = -1.5f * (finalIndex * heightColumn);
		}

		return pos;
	}

	// Get Card Position Center
	public Vector2 GetCardPositionCenter(string turnPos)
	{
		Vector2 pos = new Vector2();

		if (turnPos == "Bottom")
			pos = new Vector2 (0 , -cardHeight / 5);
		else if (turnPos == "Right")
			pos = new Vector2 (cardWidth / 3 , 0);
		else if (turnPos == "Top")
			pos = new Vector2 (0 , cardHeight / 5);
		else
			pos = new Vector2 (-cardWidth / 3 , 0);

		return pos;
	}

	// Get Card Position Used
	public Vector2 GetCardPositionUsed(string turnPos , int index)
	{
		var cardsLayoutRT = cardsLayout.GetComponent<RectTransform>();
		Vector2 pos = new Vector2();

		if (turnPos == "Bottom" || turnPos == "Top") {
			pos = new Vector2 (cardWidth, 0.5f * (cardsLayoutRT.rect.height - cardHeight) - cardHeight / 1.5f);
			pos.x -= (index - 1) * cardHeight * 0.15f;
			pos.y -= index % 2 == 0 ? cardWidth * 0.15f : 0f;
		} else {
			pos = new Vector2 (-0.5f * (cardsLayoutRT.rect.width - cardWidth) + cardWidth, cardHeight / 1.5f);
			pos.x += index % 2 == 0 ? cardWidth * 0.15f : 0f;
			pos.y -= (index - 1) * cardHeight * 0.15f;
		}

		return pos;
	}

	// Get Card Rotation With Index
	public float GetCardRotationWithIndex(int index , string turnPos)
	{
		float rotateColumn = Mathf.PI / (13 - 1);
		float rotate = -1 * Mathf.Cos ((12 - index) * rotateColumn) * rotation;

		if (turnPos == "Bottom")
			rotate += 0;
		else if (turnPos == "Right")
			rotate += 90;
		else if (turnPos == "Top")
			rotate -= 180;
		else
			rotate -= 90;

		return rotate;
	}

	// Get Card Rotation Used
	public float GetCardRotationUsed(string turnPos)
	{
		float rotate = 0;
	
		if (turnPos == "Bottom" || turnPos == "Top")
			rotate = 90;

		return rotate;
	}

	// Get Card Sacle
	public Vector3 GetCardSacle(string turnPos)
	{
		if(gameManager.tv)
			return new Vector3 (0.5f , 0.5f , 0.5f);
		else if (turnPos == "Bottom")
			return new Vector3 (1f , 1f , 1f);
		else
			return new Vector3 (0.3f , 0.3f);
	}

	// Get Turn Position
	public string GetTurnPosition(int turn)
	{
		int findTurn = GetTurnPositionNumber(turn);
		string pos;

		if (findTurn == 1)
			pos = "Bottom";
		else if (findTurn == 2)
			pos = "Right";
		else if (findTurn == 3)
			pos = "Top";
		else
			pos = "Left";

		return pos;
	}

	// Get Turn Position Number
	public int GetTurnPositionNumber(int turn)
	{
		if(!gameManager.tv)
			return ((4 + turn - gameManager.myTurn) % 4) + 1;
		else
			return turn;
	}

	// Get Turn Position Number
	public int GetTurnPositionNumberGovernorBase(int turn)
	{
		if (!gameManager.tv)
			return ((4 + turn - gameManager.governor) % 4) + 1;
		else
			return turn;
	}

    // Vector 3 Equal
    public bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }
}