
using System.Collections.Generic;
using System.Runtime.InteropServices;
//using static BlackJackSimulator;

BlackJackSimulator blackJackSimulator = new BlackJackSimulator();
blackJackSimulator.gameSetup();
blackJackSimulator.startGame();


public class BlackJackSimulator
{
    bool stillPlaying = true;
    public List<Player> currentPlayerList = new List<Player>();
    Dealer dealer = new Dealer(new List<Card>(), "Dealer", 0);
    public Deck currentDeck = new Deck();
    public Player currentPlayer = null;
    public List<Card> cardList = new List<Card>();

    public enum Suit { Heart, Diamond, Spade, Club };
    public enum Rank { Ace = 11, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10, Jack = 10, Queen = 10, King = 10 };

    public class Deck
    {
        public List<Card> deck = new List<Card>();
        private static Random rng = new Random();

        public void loadDeck(List<Card> cardList)
        {
            deck.Clear();
            deck.AddRange(cardList);
            shuffleDeck();
        }

        public void shuffleDeck()
        {
            // uses fisher-yates shuffle
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
        }
    }

    public class Card
    {
        public Rank Rank;
        public Suit Suit;
        public bool isVisible;

        public Card(Rank rank, Suit suit, bool isVisible)
        {
            Suit = suit;
            Rank = rank;
            this.isVisible = isVisible;
        }
    }

    public class Player
    {
        public List<Card> hand = new List<Card>();
        public string playerName;
        public int currentCardValue;

        public Player(List<Card> hand, string playerName, int currentCardValue)
        {
            this.hand = hand;
            this.playerName = playerName;
            this.currentCardValue = currentCardValue;
        }
    }

    public class Dealer : Player
    {
        public Dealer(List<Card> hand, string playerName, int currentCardValue) : base(hand, playerName, currentCardValue) { }
    }
    public void gameSetup()
    {
        Console.WriteLine("Beginning new game...");
        Console.WriteLine("How many players will join?");
        int p = input();
        numberOfPlayers(p, currentPlayerList);
        Console.WriteLine("Number of players:" + Convert.ToString(countPlayers(currentPlayerList)));
        currentPlayer = currentPlayerList[0];
        generateCards(cardList);
        currentDeck.loadDeck(cardList);
        Console.WriteLine("Starting deck size: " + Convert.ToString(currentDeck.deck.Count));
        dealHands(currentPlayerList, currentDeck);
        Console.WriteLine("Hands have been dealt. What would you like to do? Please enter a number value for a command");
        Console.WriteLine("Current deck size: " + Convert.ToString(currentDeck.deck.Count));

    }

    public void startGame()
    {
        while (stillPlaying == true)
        {
            // show all visible cards in play on the board;
            showPlayerCardValues(currentPlayerList);
            Console.WriteLine("1.Hit\n2.Stay\n3.Surrender");
            switch (Convert.ToInt32(Console.ReadLine()))
            {
                case 1:
                    hit(currentPlayer, currentDeck);
                    break;
                case 2:
                    // no change, pass to next player;
                    stay();
                    dealerTurn(currentPlayerList.Find(x => x.playerName == "Dealer"), currentDeck);
                    break;
                case 3:
                    surrender();
                    break;
                default:
                    Console.WriteLine("Invalid command. Please enter 1, 2, or 3.");
                    break;

            }
        }
    }

    public void generateCards(List<Card> cardlist)
    {
        foreach (var suit in Enum.GetValues<Suit>())
        {
            foreach (var rank in Enum.GetValues<Rank>())
            {
                //add new card combination to the cardlist
                cardList.Add(new Card(rank, suit, false));
            }
        }
    }

    public void dealHands(List<Player> currentPlayerList, Deck currentDeck)
    {
        foreach (var player in currentPlayerList)
        {
            player.hand.Clear();
            player.hand.Add(currentDeck.deck[0]);
            currentDeck.deck.RemoveAt(0);
        }
        foreach (var player in currentPlayerList)
        {
            //still need to check that this portion of code works correctly
            currentDeck.deck[0].isVisible = true;
            player.hand.Add(currentDeck.deck[0]);
            //set this card to visible
            currentDeck.deck.RemoveAt(0);
        }
    }

    public void hit(Player currentPlayer, Deck currentDeck)
    {
        Console.WriteLine("Adding another card to turn players hand");
        currentPlayer.hand.Add((currentDeck.deck[0]));
        currentDeck.deck.RemoveAt(0);
        calculatePlayerCardValue(currentPlayer);
        Console.WriteLine("Current deck size: " + Convert.ToString(currentDeck.deck.Count));

        if (currentPlayer.currentCardValue == 21)
        {
            calculatePlayerCardValue(currentPlayer);
            Console.WriteLine("Current deck size: " + Convert.ToString(currentDeck.deck.Count));
            Console.WriteLine("Congratulations! You Win!");
            stillPlaying = false;
           
        }
        else if (currentPlayer.currentCardValue > 21)
        {
            Console.WriteLine("You exceeded 21!");
            showPlayerCardValues(currentPlayerList);
            stillPlaying = false;
            
        }
    }

    public void stay()
    {
        Console.WriteLine("Turn player chooses to stay");
    }

    public void surrender()
    {
        //withdraw from game, if only player, dealor wins and exit game
        if (currentPlayerList.Count == 1)
        {
            Console.WriteLine("Player forefeits. Dealor Wins!");
            Environment.Exit(0);
        }
    }

    public int calculatePlayerCardValue(Player currentPlayer)
    {
        currentPlayer.currentCardValue = 0;
        foreach (var card in currentPlayer.hand)
        {
            //get the currentcardvalue of the players hand, using default ace values of 11
            currentPlayer.currentCardValue += Convert.ToInt32(card.Rank);
        }
        //if the hand value is over 21, check again for any aces, and reduce the players hand value by 10 for each one until the player is at or below 21
        if (currentPlayer.currentCardValue > 21)
        {
            foreach (var card in currentPlayer.hand)
            {
                if (card.Rank == Rank.Ace)
                {
                    currentPlayer.currentCardValue -= 10;
                    if (currentPlayer.currentCardValue <= 21)
                    {
                        break;
                    }
                }
            }
        }
        return currentPlayer.currentCardValue;
    }

    public void showPlayerCardValues(List<Player> currentPlayerList)
    {
        foreach (var player in currentPlayerList)
        {
            player.currentCardValue = 0;
            foreach (var card in player.hand)
            {
                // currently the value of all cards will be shown, even if not visible for testing purposes.
                player.currentCardValue += Convert.ToInt32(card.Rank);

                Console.WriteLine("Card: " + Convert.ToString(Convert.ToInt32(card.Rank)) + " of " + Convert.ToString(card.Suit) + "s");
            }
            if (player.currentCardValue > 21)
            {
                foreach (var card in player.hand)
                {
                    if (card.Rank == Rank.Ace)
                    {
                        player.currentCardValue -= 10;
                        if (player.currentCardValue <= 21)
                        {
                            break;
                        }
                    }
                }
            }
            Console.WriteLine("Player Name: " + player.playerName + ". Total Card Value: " + player.currentCardValue);
        }
    }

    public int numberOfPlayers(int numberOfPlayers, List<Player> currentPlayerList)
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            string playerNumber = (i + 1).ToString();
            Player player = new Player(new List<Card>(), "Player" + playerNumber, 0);
            currentPlayerList.Add(player);
        }
        currentPlayerList.Add(dealer);
        return numberOfPlayers;
    }
    public int countPlayers(List<Player> currentPlayerList)
    {
        int count = 0;
        foreach (var player in currentPlayerList)
        {
            count += 1;
        }
        return count;
    }

    public void dealerTurn(Player dealer, Deck currentDeck)
    {
        if (dealer.currentCardValue <= 16)
        {
            while (dealer.currentCardValue <= 16)
            {
                hit(dealer, currentDeck);
            }
        }
        else if (dealer.currentCardValue > 16)
        {
            stay();
            return;
        }
    }
    public int input()
    {
        try
        {
            return Convert.ToInt32(Console.ReadLine());
        }
        catch (Exception)
        {
            Console.WriteLine("Answer must be a numerical input. Please Try Again");
            return input();
        }
    }
}