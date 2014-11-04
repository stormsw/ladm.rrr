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
	And Current transaction has party "Tim Taler" with role "Grantee" associated with LAUnit "Occupancy4Merge-LA-01"
	And	Registration transaction "REGO" with No."TRN-OCCUPANCY-4-MERGE-002" is set current
	And Current transaction has target LAUnit "Occupancy4Merge-LA-02"
	And Current transaction has property with Uid = "Property 4 Merge 02" in LAUnit "Occupancy4Merge-LA-02"
	And Current transaction has party "Steve Adams" with role "Grantee" associated with LAUnit "Occupancy4Merge-LA-02"	
	When transaction "TRN-OCCUPANCY-4-MERGE-001" is completed
	And transaction "TRN-OCCUPANCY-4-MERGE-002" is completed
@merger
Scenario: Simple merge of 2 proeprties
	Given Registration transaction "MERG" with No."TRN-MERGER" is set current
	And Transaction No"TRN-MERGE-001" has party "Mark Twen (merger)" with role "Grantee" associated with LAUnit "Merge-LA-01"