# LtlSharp Roadmap

## Probabilistic Systems

Done:

* Markov Chains
  * Reachability Probabilities
  * Transient Reachability Probabilities
  * Qualitative Properties
* Probabilistic Computation Tree Logic
  * PCTL Model Checking
* Linear-time properties 
* Model-Checking PCTL*

To do:

* Markov Decision Processes
  * Reachability Probabilities
  * PCTL Model Checking
  * Limiting Properties
  * Linear-Time Properties and PCTL*

Future work

* Markov chains with costs
* Markov Decision Processes
  * Fairness

## Refactoring

* Implement w-automata as parent of all automata
* Ensure that all automata exports to DOT
* Removes multiple initial nodes in automata (except MC)
* Represents set of literals as BDD so operations for combining/testing are easier and faster.