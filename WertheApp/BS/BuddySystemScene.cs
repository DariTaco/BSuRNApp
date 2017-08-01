using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using CocosDenshion;
using BinaryTree; //Documentation see: https://github.com/Marusyk/BinaryTree
				  /*Available operations:

				  void Add(T value) - adds a new element to the tree
				  int Count - returns count of elements in tree
				  bool IsReadOnly - always return false
				  bool Contains(T value) - checks if the tree contains the element
				  bool Remove(T value) - remove element from the tree. Returns true if element was removed.
				  void Clear() - clears tree
				  void CopyTo(T[] array, int arrayIndex) - copies all the elements of the tree to the specified one-dimensional array starting at the specified destination array index.
				  void SetTraversalStrategy(TraversalStrategy traversalStrategy) - sets type of traversal(Pre-order, In-order, Post-order)
				  IEnumerator<T> GetEnumerator() - returns numerator of tree
				  To display all elements of tree, use:

				  foreach (var item in binaryTree)
				  {
					 Console.Write(item + " ");
				  }*/

using Xamarin.Forms;

namespace WertheApp.BS
{
    public class BuddySystemScene : CCScene
    {
		//VARIABLES
		CCLayer layer;

        CCDrawNode cc_box;

		int absoluteMemorySize;
        int powerOfTwo;

        public static List<int> freeBlocksList; //Process names
        public static BinaryTree<int> binaryTree;

		//CONSTRUCTOR
		public BuddySystemScene(CCGameView gameView): base(gameView)
        {

            absoluteMemorySize = Int32.Parse(BuddySystem.absoluteMemorySize.ToString());
            powerOfTwo = BuddySystem.powerOfTwo;

            //set up the binary tree
            binaryTree = new BinaryTree<int>();
            var postOrder = new PostOrderTraversal(); // we probably use this one
			var inOrder = new InOrderTraversal();
			var preOrder = new PreOrderTraversal();
            binaryTree.SetTraversalStrategy(postOrder); //set the strategy to postOrder
            binaryTree.Add(absoluteMemorySize);

			foreach (var item in binaryTree)
			{
                Debug.WriteLine(item + " ");
			}

			//add a layer to draw on
			layer = new CCLayer();
			this.AddLayer(layer);


            DrawTest();
		}

        //METHODS
        void DrawTest()
        {
			var box = new CCRect(15, 21, 302, 50);//CCRect(x,y,legth,width)
			cc_box = new CCDrawNode();
			cc_box.DrawRect(
				box,
				fillColor: CCColor4B.White,
				borderWidth: 1,
				borderColor: CCColor4B.Gray);
			//add box to layer
			layer.AddChild(cc_box);
        }

        //draws the current Memory
        void DrawMemory()
        {
            
        }

		//Finds a free block of size 2k and marks it as occupied
        void Allocate(int k)
        {
            
        }

		//Marks the previously allocated block B as free and may merge it with others to form a larger free block
        void Dellocate(String b)
        {
            
        }

        //Merges to blocks
        void MergeBlocks()
        {
            
        }

	}
}

