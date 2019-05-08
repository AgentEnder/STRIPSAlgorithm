# Project Report: A STRIPS Attempt.
## Input Formatting.

When running this AI, place a file named "test{x}.in" where x is the number of the test case. Upon running the program, enter the number for your test file and press enter.

The program than parses that file and starts the algorithm.

The anticipated file structure is as follows:

"""
0 0
a 0
b c

0 b
0 a
0 c
"""

The section before the blank line is the starting state, and the section beneath it is the goal state. Zeros represent a blank space in the world.

The program assumes correct input, so try to not leave any blocks floating in starting or goal states.
## Data Structures.
Using C#, our implementation relys on 2 main generic data types, and a struct that is used to represent operators.
We rely on a Dictionary to map predicate strings to the action struct in our STRIPS function, and the List<t> generic in several locations as an extensible container type.

### Operator Struct.
Our operators are represented by a struct with 4 fields.
1. string name;
2. List<string> pre //The preconditions needed for our operator to fire
3. List<string> add //What is added by our operator
4. List<string> del //What is removed when our operator is fired.
 
### Operator Factories
To build all of our operator instances, 4 functions are used. These functions take the operators arguments and output an instance of our operator struct that is fully filled out. Using these allows our implentation to not be limited to a certain number of block names.

## Implementation
### File Parsing
The input file is read to a queue of strings, where each element is a line in the file.
1. A Y variable is mapped to zero at the start
1. The queue is popped. This gets a line of the file, which is than split on spaces. X is mapped to zero now.
1. Iterate through the split line, incrementing X after each element. Store the elements in a 2D char array, saving them at position [x,y]
1. Iterate Y.
1. Repeat steps 2->4 until the line is blank. Save the character map as your starting state.
1. Repeat steps 1->4 until end-of-file. Save this map as the goal state.

### Predicate Parsing
This is the process used to transfer a 2D character map into a predicate world.
1. Initialize a List<String> to hold our predicates
1. Start at the bottom Y coordinate, iterating up.
1. Any character read here is on the table, so add "OnTable{x}" where x is the character read, as long as x is not 0 (our empty space)
1. Iterate up in the character map.
2. Any characters read here must be on top of something. Add "On({x},{y})" to the world where x is the current character, and y is the character at the same x position as our x on the previous layer.
3. Repeate steps 4 and 5 until you reach the top-most y coordinate.
 
### STRIPS Algorithm
This algorithm works as follows, to build a plan:

1. Initialize a Stack<string> to hold our goals.
2. Fill it with the predicates in our goal state.
3. Initialize a List<string> to hold our world state.
4. Fill it with the starting predicates.
5. Repeat the following process until the goal stack is empty:
   1. Get the top element, x.
   1. If x is an Operator remove everything that is in its del field, and add everything in its add field. Add X to the plan.
   2. If x is a predicate:
       1. If the world contains it pop the stack.
       2. Else:
           1. Look through every operator and score them based on whether they would add x to our world, and what their side effects would be.
           2. Choose an operator, y, that adds this predicate.
           3. Add it to our goal stack, along with its preconditions.
4. We now should have our plan.
## Results.

This project is capable of solving some block world problems using the STRIPS algorithm. It is only a partial solution due to issues with looping that are currently unsolved during certain problems. Test case 2 is an example of a file which gives issues. 