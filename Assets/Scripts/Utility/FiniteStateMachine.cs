using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FiniteStateMachine<Tstate, Talphabet> {
	public Tstate current;
    private Func<Talphabet, Tstate> transition;
    private Func<Tstate, Func<Talphabet, Tstate>> rulebase;

	public FiniteStateMachine(Tstate start, Func<Tstate, Func<Talphabet, Tstate>> rules) {
		this.current = start;
        this.rulebase = rules;
        this.transition = rules(start);
    }

	public Tstate Input(Talphabet input) {
		current = transition(input);
        transition = rulebase(current);
        return current;
    }

}
