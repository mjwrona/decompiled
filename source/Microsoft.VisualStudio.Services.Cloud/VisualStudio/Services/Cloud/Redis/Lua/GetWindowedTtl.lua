-- KEYS[i] - Redis key to be retrieved

local result = {}

for i=1,#KEYS do
  result[i] = redis.call('PTTL',KEYS[i])
end  

return result
