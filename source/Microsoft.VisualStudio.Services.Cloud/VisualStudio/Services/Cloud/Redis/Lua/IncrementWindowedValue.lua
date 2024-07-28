-- KEYS[i] - Redis key to be incremented
-- ARGV[i] - Redis values to be processed for the key (two per key: 2*i-1 - increment, 2*i - maximum)
-- ARGV[N*2+1] - Window duration (where N is the number of keys)

local keys = #KEYS
local duration = tonumber(ARGV[#ARGV])

-- process the arguments and return the results
local result = {}
-- get all values for the requested keys
local values = redis.call('MGET',unpack(KEYS))

local vi = 1 -- vi is the increment
             -- vi+1 is the value to cap our key at, -1 means no limit
for ki=1,keys do
  local value = 0
  if values[ki] then
    local ttl = redis.call('PTTL',KEYS[ki])
	value = tonumber(values[ki])

    -- determine how much we should replenish for this key
    local passed = duration - ttl
    local replenishment = passed / duration * value

    -- replenish
    value = math.max(value - replenishment, 0)
  end

  -- increment the value
  value = value + math.max(ARGV[vi], 0)

  -- cap the value if a maximum is set
  local maximum = tonumber(ARGV[vi+1])
  if maximum >= 0 then
    value = math.min(maximum, value)
  end
  
  -- set the key elements and update the expiration
  -- tostring is required here or redis will drop decimal
  redis.call('SET',KEYS[ki],tostring(value),'PX',duration)

  -- round up the current value before returning it as a result
  result[ki] = math.ceil(value)

  -- move our value index to the next set of values
  vi = vi + 2
end

return result
