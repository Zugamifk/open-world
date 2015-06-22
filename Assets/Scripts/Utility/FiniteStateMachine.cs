using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FiniteStateMachine<Tstate, Talphabet>
where Talphabet : IComparable
where Tstate : IComparable
 {
	public Tstate current;
    private Func<Talphabet, Tstate> transition;
    private Func<Tstate, Func<Talphabet, Tstate>> rulebase;
	public Func<string> lastError = ()=>"No error";
    private FSM_FLAG error;

    public FiniteStateMachine(Tstate start, Func<Tstate, Func<Talphabet, Tstate>> rules) {
		this.current = start;
        this.rulebase = rules;
        this.transition = rules(start);
    }

	public void Input(Talphabet input) {
		var newState = transition(input);

		if (newState.CompareTo(default(Tstate))==0) {
			SetErrorFlag(FSM_FLAG.DID_NOT_TRANSITION, current.ToString(), input.ToString());
		} else if (newState.CompareTo(current)==0) {
			SetErrorFlag(FSM_FLAG.SAME_STATE);
		}

        transition = rulebase(current);

		if(transition==null) {
			SetErrorFlag(FSM_FLAG.SAME_STATE, newState.ToString());
		}

		current = newState;
		// Debug.Log(current);
    }

	private void SetErrorFlag(FSM_FLAG flag, params string[] args) {
        error = flag;
    	switch(flag) {
			case FSM_FLAG.UNSET:
				lastError = () => "No error";
				break;
			case FSM_FLAG.SAME_STATE:
				lastError = () => "Transition looped back";
				break;
			case FSM_FLAG.DID_NOT_TRANSITION:
				lastError = () => "No transition from {0} for input {1}".Format(args);
				break;
			case FSM_FLAG.NO_TRANSITION_RULE:
				lastError = () => "State {0} is a final state".Format(args);
				break;
        }
	}

	private enum FSM_FLAG {
		UNSET = 0,
		SAME_STATE, // State did not change (loopback)
		DID_NOT_TRANSITION, // transition rule returned null (no transition for given input)
		NO_TRANSITION_RULE // State changed, but rulebase has no transition rules for current state
	}
}
