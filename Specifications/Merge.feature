Feature: Merge
	It happens when set of source properties produces single (or smaller result set) property
	{o1,o2,o3} => {o1`}, {o1,o2,o3} => {o1`,o2`} etc
	We may inherit holders or introduce new ones - it doesn't matter
	Technicall all source RRRs are archived and target produced. 
	The Merge operation may have as a result an origin property elimenated.

Background: there 2 properties for a different right-holders are exists.
	Given Registration transaction "REGO" with No."TRN-OCCUPANCY-4-MERGE-001" is set current
	And Current transaction has target LAUnit "Occupancy4Merge-LA-01"
	And Current transaction has property with Uid = "Property 4 Merge 01" in LAUnit "Occupancy4Merge-LA-01"
	//generat
	And Current transaction has party "Tim Taler" with role "Grantee" associated with LAUnit "Occupancy4Merge-LA-01"
	And Current transaction target parties reference property with Uid = "Occupancy4Merge-LA-01"
@merger
Scenario: Simple merge of 2 proeprties
	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen
