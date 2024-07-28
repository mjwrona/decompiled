-- KEYS[i] - Redis key to fetch TTL for

local result = {}

for i=1,#KEYS do
  result[i] = redis.call('PTTL',KEYS[i])
end

return result  
