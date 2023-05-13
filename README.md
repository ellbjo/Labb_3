#How to get all persons
https://localhost:7155/api/persons
PersonListDto is used

#How to get all descriptions by personId
https://localhost:7155/api/getInterestsByPersonId/1
InterestListDto is used.

#How to get all links by personId
https://localhost:7155/api/getLinksByPersonId/1


#How to add interest to person by personID
https://localhost:7155/api/addNewInterestOnPerson?PersonId=2&InterestTitle=Simma&InterestDesciption=Simma är roligt
Parameters are PersonId, InterestTitle and InterestDescription
AddInterestToPersonDTO is used

#How to add a new link on a interest
https://localhost:7155/api/addLinkOnInterest?InterestId=4&Link=aftonbladet.se
Parameters are InterestId and Link 
AddInterestLinkDto is used