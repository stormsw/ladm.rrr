Feature: Subdivision
		It happened when single or some set of source properties produces new set
		It may vary in results depends on lifespan of origin 
		whethere it (o)=>{o`,n} or (o)=>{n1,n2} or {o1,o2} => {o1`,o2`,n...}
		But main idea - cancel all RRR on source and produce new for targets
		We may work on LAUnits for different right types at the same operation

Background: 
	Given We have Parcel with Uid = "Property 2"
	#/// REGO - Registration of Occupancy right to Grantee - see Initializer for details	 
	And Registration transaction "REGO" with No."TRN-002" is set current
	#/// LAUnits represents interests to be registered proto
	And Transaction No"TRN-002" has target LAUnit "Occupancy-LA-02"
	And Transaction No"TRN-002" has property with Uid = "Property 2" in LAUnit "Occupancy-LA-02"
	And Transaction No"TRN-002" has party "Tim Taler" with role "Grantee" associated with LAUnit "Occupancy-LA-02"
	And Transaction No"TRN-002" has party "Sara Taler" with role "Grantee" associated with LAUnit "Occupancy-LA-02"
	# same result as prev both
	#And Transaction No"TRN-002" target parties reference property with Uid = "Property 2"
	When transaction "TRN-002" is completed

@subdivision
Scenario: Subdivision produces 2 new properties and no additional interests registered on source
		  Source property is archived as a result

	Given Registration transaction "SUBS" with No."TRN-003" is set current
	And Transaction No"TRN-003" has source LAUnit "Occupancy-LA-02"
	And Transaction No"TRN-003" has target LAUnit "Occupancy-LA-03"
	And Transaction No"TRN-003" has target LAUnit "Occupancy-LA-04"
	And Transaction No"TRN-003" has property with Uid = "Property 2" in LAUnit "Occupancy-LA-02"
	And Property "Property 2" in Transaction No"TRN-003" Status set "Archived"
	And Transaction No"TRN-003" has property with Uid = "Property 3" in LAUnit "Occupancy-LA-03"
	And Transaction No"TRN-003" has property with Uid = "Property 4" in LAUnit "Occupancy-LA-04"
	And Transaction No"TRN-003" has party "Tim Taler" with role "Grantee" associated with LAUnit "Occupancy-LA-03"
	And Transaction No"TRN-003" has party "Sara Taler" with role "Grantee" associated with LAUnit "Occupancy-LA-04"
	When transaction "TRN-003" is completed
	Then Cancelled by "TRN-003" 2 "Occupancy" rights
	And Party "Tim Taler" have active "Occupancy" rights on "Property 3"
	And Party "Sara Taler" have active "Occupancy" rights on "Property 4"
	And Property "Property 2" is archived
